using Faktur.Contracts.Articles;
using Faktur.Contracts.Banners;
using Faktur.Contracts.Departments;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Contracts.Stores;
using Faktur.Contracts.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Faktur.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors;
  private readonly Actor _system = Actor.System;

  public Mapper()
  {
    _actors = [];
  }
  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId actorId = new(actor.Id);
      _actors[actorId] = actor;
    }
  }

  public static Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Id = new ActorId(source.Id).ToGuid(),
    Type = source.Type,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public Article ToArticle(ArticleEntity source)
  {
    Article destination = new(source.DisplayName)
    {
      Gtin = source.Gtin,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Banner ToBanner(BannerEntity source)
  {
    Banner destination = new(source.DisplayName)
    {
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Department ToDepartment(DepartmentEntity source, Store store)
  {
    Department destination = new(store, source.Number, source.DisplayName)
    {
      Description = source.Description,
      CreatedBy = FindActor(source.CreatedBy),
      CreatedOn = AsUniversalTime(source.CreatedOn),
      UpdatedBy = FindActor(source.UpdatedBy),
      UpdatedOn = AsUniversalTime(source.UpdatedOn)
    };

    return destination;
  }

  public Product ToProduct(ProductEntity source)
  {
    if (source.Article == null)
    {
      throw new ArgumentException($"The '{nameof(source.Article)}' is required.", nameof(source));
    }
    if (source.Store == null)
    {
      throw new ArgumentException($"The '{nameof(source.Store)}' is required.", nameof(source));
    }

    Article article = ToArticle(source.Article);
    Store store = ToStore(source.Store, includeDepartments: true);
    Product destination = new(article, store)
    {
      Sku = source.Sku,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Flags = source.Flags,
      UnitPrice = source.UnitPrice,
      UnitType = source.UnitType
    };
    if (source.Department != null)
    {
      destination.Department = ToDepartment(source.Department, store);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Receipt ToReceipt(ReceiptEntity source, bool includeItems)
  {
    if (source.Store == null)
    {
      throw new ArgumentException($"The '{nameof(source.Store)}' is required.", nameof(source));
    }

    Store store = ToStore(source.Store, includeDepartments: true);
    Receipt destination = new(store)
    {
      IssuedOn = AsUniversalTime(source.IssuedOn),
      Number = source.Number,
      ItemCount = source.ItemCount,
      SubTotal = source.SubTotal,
      Total = source.Total,
      HasBeenProcessed = source.HasBeenProcessed,
      ProcessedBy = TryFindActor(source.ProcessedBy),
      ProcessedOn = AsUniversalTime(source.ProcessedOn)
    };

    if (includeItems)
    {
      foreach (ReceiptItemEntity item in source.Items)
      {
        destination.Items.Add(ToReceiptItem(item, destination));
      }
    }

    foreach (ReceiptTaxEntity tax in source.Taxes)
    {
      destination.Taxes.Add(new ReceiptTax(tax.Code, tax.Flags)
      {
        Rate = tax.Rate,
        TaxableAmount = tax.TaxableAmount,
        Amount = tax.Amount
      });
    }

    MapAggregate(source, destination);

    return destination;
  }
  public ReceiptItem ToReceiptItem(ReceiptItemEntity source, Receipt receipt)
  {
    ReceiptItem destination = new(receipt, source.Label)
    {
      Number = (ushort)source.Number,
      Gtin = source.Gtin,
      Sku = source.Sku,
      Label = source.Label,
      Flags = source.Flags,
      Quantity = source.Quantity,
      UnitPrice = source.UnitPrice,
      Price = source.Price,
      Category = source.Category,
      CreatedBy = FindActor(source.CreatedBy),
      CreatedOn = AsUniversalTime(source.CreatedOn),
      UpdatedBy = FindActor(source.UpdatedBy),
      UpdatedOn = AsUniversalTime(source.UpdatedOn)
    };

    if (source.DepartmentNumber != null && source.DepartmentName != null)
    {
      destination.Department = new DepartmentSummary(source.DepartmentNumber, source.DepartmentName);
    }

    return destination;
  }

  public Store ToStore(StoreEntity source, bool includeDepartments)
  {
    Store destination = new(source.DisplayName)
    {
      Number = source.Number,
      Description = source.Description,
      DepartmentCount = source.DepartmentCount
    };

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new Address(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = AsUniversalTime(source.AddressVerifiedOn),
        IsVerified = source.IsAddressVerified
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new Email(source.EmailAddress)
      {
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = AsUniversalTime(source.EmailVerifiedOn),
        IsVerified = source.IsEmailVerified
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new Phone(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        VerifiedBy = TryFindActor(source.PhoneVerifiedBy),
        VerifiedOn = AsUniversalTime(source.PhoneVerifiedOn),
        IsVerified = source.IsPhoneVerified
      };
    }

    if (source.Banner != null)
    {
      destination.Banner = ToBanner(source.Banner);
    }

    if (includeDepartments)
    {
      foreach (DepartmentEntity department in source.Departments)
      {
        destination.Departments.Add(ToDepartment(department, destination));
      }
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Tax ToTax(TaxEntity source)
  {
    Tax destination = new(source.Code, source.Rate)
    {
      Flags = source.Flags
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor? TryFindActor(string? id) => id == null ? null : FindActor(new ActorId(id));
  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;

  private static DateTime? AsUniversalTime(DateTime? value) => value.HasValue ? AsUniversalTime(value.Value) : null;
  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Local => value.ToUniversalTime(),
    _ => value,
  };
}
