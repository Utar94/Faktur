using AutoMapper;
using Faktur.Core.Articles;
using Faktur.Core.Products;
using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Parsing;
using Faktur.Core.Settings;
using Faktur.Core.Stores;
using Logitar;
using Logitar.Identity.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Faktur.Core.Receipts.Commands
{
  public class ImportReceiptHandler : IRequestHandler<ImportReceipt, ReceiptModel>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IReceiptParser parser;
    private readonly TaxesSettings taxesSettings;
    private readonly IUserContext userContext;

    public ImportReceiptHandler(
      IDbContext dbContext,
      IMapper mapper,
      IReceiptParser parser,
      TaxesSettings taxesSettings,
      IUserContext userContext
    )
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
      this.taxesSettings = taxesSettings ?? throw new ArgumentNullException(nameof(taxesSettings));
      this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<ReceiptModel> Handle(ImportReceipt request, CancellationToken cancellationToken)
    {
      Store store = await dbContext.Stores
        .Include(x => x.Products).ThenInclude(x => x.Article)
        .SingleOrDefaultAsync(x => x.Id == request.Payload.StoreId, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(request.Payload.StoreId, nameof(request.Payload.StoreId));

      Dictionary<long, Article> articles = await dbContext.Articles
        .Where(x => x.Gtin.HasValue)
        .ToDictionaryAsync(x => x.Gtin!.Value, x => x, cancellationToken);

      Dictionary<string, Department> departments = await dbContext.Departments
        .Where(x => x.StoreId == store.Id && x.Number != null)
        .ToDictionaryAsync(x => x.Number!, x => x, cancellationToken);

      Dictionary<int, Product> productsByArticle = store.Products
        .ToDictionary(x => x.ArticleId, x => x);
      Dictionary<string, Product> productsBySku = store.Products
        .Where(x => x.Sku != null)
        .ToDictionary(x => x.Sku!, x => x);

      var receipt = new Receipt(store, userContext.Id)
      {
        IssuedAt = request.Payload.IssuedAt ?? DateTime.UtcNow,
        Number = request.Payload.Number?.CleanTrim()
      };

      var gst = new Tax(receipt)
      {
        Code = Constants.GstCode,
        Rate = taxesSettings.Gst
      };
      receipt.Taxes.Add(gst);

      var qst = new Tax(receipt)
      {
        Code = Constants.QstCode,
        Rate = taxesSettings.Qst
      };
      receipt.Taxes.Add(qst);

      CultureInfo culture = request.Payload.Culture == null
        ? CultureInfo.InvariantCulture
        : CultureInfo.GetCultureInfo(request.Payload.Culture);
      IEnumerable<LineInfo> lines = parser.Execute(culture, request.Payload.Lines);
      foreach (LineInfo line in lines)
      {
        Article? article = null;
        Department? department = null;
        Product? product = null;

        if (line.Department != null)
        {
          if (!departments.TryGetValue(line.Department.Number, out department))
          {
            department = new Department(store, userContext.Id)
            {
              Name = line.Department.Name,
              Number = line.Department.Number
            };
            departments.Add(department.Number, department);

            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync(cancellationToken);
          }
        }

        string? sku = null;
        long? gtin = ParseGtin(line.Id);
        if (gtin.HasValue)
        {
          if (articles.TryGetValue(gtin.Value, out article))
          {
            productsByArticle.TryGetValue(article.Id, out product);
          }
        }
        else
        {
          sku = line.Id;
          productsBySku.TryGetValue(sku, out product);
        }

        if (product == null)
        {
          if (article == null)
          {
            article = new Article(userContext.Id)
            {
              Gtin = gtin,
              Name = line.Label ?? line.Id
            };
            dbContext.Articles.Add(article);

            if (gtin.HasValue)
            {
              articles.Add(gtin.Value, article);
            }
          }

          product = new Product(article, store, userContext.Id)
          {
            Department = department,
            DepartmentId = department?.Id,
            Flags = line.Flags,
            Label = line.Label,
            Sku = sku,
            UnitPrice = line.UnitPrice
          };
          article.Products.Add(product);

          if (sku != null)
          {
            productsBySku.Add(sku, product);
          }

          await dbContext.SaveChangesAsync(cancellationToken);

          productsByArticle.Add(article.Id, product);
        }

        receipt.Items.Add(new Item(product, receipt)
        {
          Price = line.Price,
          Quantity = line.Quantity ?? 1,
          UnitPrice = line.UnitPrice ?? line.Price
        });

        if (product.Flags?.Contains('F') == true)
        {
          gst.TaxableAmount += line.Price;
        }
        if (product.Flags?.Contains('P') == true)
        {
          qst.TaxableAmount += line.Price;
        }
      }

      dbContext.Receipts.Add(receipt);
      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ReceiptModel>(receipt);
    }

    private static long? ParseGtin(string s)
    {
      if (long.TryParse(s, out long gtin) && gtin >= 0 && gtin <= 99999999999999)
      {
        return gtin;
      }

      return null;
    }
  }
}
