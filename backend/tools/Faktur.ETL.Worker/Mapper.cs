using Faktur.Contracts.Articles;
using Faktur.Contracts.Banners;
using Faktur.Contracts.Departments;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Contracts.Stores;
using Faktur.ETL.Worker.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Faktur.ETL.Worker;

internal class Mapper
{
  private readonly Dictionary<Guid, Actor> _actors;

  public Mapper()
  {
    _actors = [];
  }

  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      _actors[actor.Id] = actor;
    }
  }

  public Article ToArticle(ArticleEntity source)
  {
    Article destination = new(source.Name)
    {
      Gtin = source.Gtin?.ToString(),
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Banner ToBanner(BannerEntity source)
  {
    Banner destination = new(source.Name)
    {
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Department ToDepartment(DepartmentEntity source, Store store)
  {
    if (source.Number == null)
    {
      throw new ArgumentException($"The {nameof(source.Number)} is required.", nameof(source));
    }

    return new Department(store, source.Number, source.Name)
    {
      Description = source.Description,
      CreatedBy = FindActor(source.CreatedById),
      CreatedOn = AsUniversalTime(source.CreatedAt),
      UpdatedBy = FindActor(source.UpdatedById ?? source.CreatedById),
      UpdatedOn = AsUniversalTime(source.UpdatedAt ?? source.CreatedAt)
    };
  }

  public Product ToProduct(ProductEntity source)
  {
    Article article = ToArticle(source.Article ?? throw new ArgumentException($"The {nameof(source.Article)} is required.", nameof(source)));
    Store store = ToStore(source.Store ?? throw new ArgumentException($"The {nameof(source.Store)} is required.", nameof(source)));

    Product destination = new(article, store)
    {
      Sku = source.Sku,
      DisplayName = source.Label,
      Description = source.Description,
      Flags = source.Flags,
      UnitPrice = source.UnitPrice,
      UnitType = source.UnitType == null ? null : Enum.Parse<UnitType>(source.UnitType)
    };

    if (source.Department != null && !source.Department.Deleted && source.Department.Number != null)
    {
      destination.Department = ToDepartment(source.Department, store);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Receipt ToReceipt(ReceiptEntity source)
  {
    Store store = ToStore(source.Store ?? throw new ArgumentException($"The {nameof(source.Store)} is required.", nameof(source)));

    Receipt destination = new(store)
    {
      IssuedOn = AsUniversalTime(source.IssuedAt),
      Number = source.Number,
      SubTotal = source.SubTotal,
      Total = source.Total,
      HasBeenProcessed = source.Processed,
      ProcessedBy = source.ProcessedById.HasValue ? FindActor(source.ProcessedById.Value) : null,
      ProcessedOn = source.ProcessedAt.HasValue ? AsUniversalTime(source.ProcessedAt.Value) : null
    };

    MapAggregate(source, destination);

    ushort number = 0;
    foreach (ReceiptItemEntity item in source.Items)
    {
      ProductEntity? product = item.Product;
      if (product != null && product.Article != null && !product.Deleted)
      {
        string label = product.Label ?? product.Article.Name;
        ReceiptItem receiptItem = new(destination, label)
        {
          Number = number,
          Gtin = product.Article.Gtin?.ToString(),
          Sku = product.Sku,
          Flags = product.Flags,
          Quantity = item.Quantity,
          UnitPrice = item.UnitPrice,
          Price = item.Price,
          Category = item.Line?.Category,
          CreatedBy = destination.CreatedBy,
          CreatedOn = destination.CreatedOn,
          UpdatedBy = destination.UpdatedBy,
          UpdatedOn = destination.UpdatedOn
        };
        if (product.Department != null && product.Department.Number != null && !product.Department.Deleted)
        {
          receiptItem.Department = new DepartmentSummary(product.Department.Number, product.Department.Name);
        }

        destination.ItemCount++;
        destination.Items.Add(receiptItem);
        number++;
      }
    }

    foreach (ReceiptTaxEntity tax in source.Taxes)
    {
      destination.Taxes.Add(new ReceiptTax(tax.Code, flags: string.Empty)
      {
        Rate = tax.Rate,
        TaxableAmount = tax.TaxableAmount,
        Amount = tax.Amount
      });
    }

    return destination;
  }

  public Store ToStore(StoreEntity source)
  {
    Store destination = new(source.Name)
    {
      Number = source.Number,
      Description = source.Description
    };

    if (source.Banner != null && !source.Banner.Deleted)
    {
      destination.Banner = ToBanner(source.Banner);
    }

    if (source.Address != null && source.City != null && source.Country != null)
    {
      destination.Address = new Address(source.Address, source.City, source.PostalCode, source.State, source.Country, formatted: string.Empty);
    }
    if (source.Phone != null)
    {
      destination.Phone = new Phone(countryCode: null, source.Phone, extension: null, e164Formatted: string.Empty);
    }

    foreach (DepartmentEntity department in source.Departments)
    {
      if (!department.Deleted && department.Number != null)
      {
        destination.DepartmentCount++;
        destination.Departments.Add(ToDepartment(department, destination));
      }
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = source.Key;
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedById);
    destination.CreatedOn = AsUniversalTime(source.CreatedAt);

    destination.UpdatedBy = FindActor(source.UpdatedById ?? source.CreatedById);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedAt ?? source.CreatedAt);
  }

  private Actor FindActor(Guid id) => _actors.TryGetValue(id, out Actor? actor) ? actor : Actor.System;

  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Utc => value,
    _ => throw new NotSupportedException($"The date time kind '{value.Kind}' is not supported."),
  };
}
