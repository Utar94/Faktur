using Logitar.Identity.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Commands
{
  public class ProcessReceiptHandler : IRequestHandler<ProcessReceipt>
  {
    private readonly IDbContext dbContext;
    private readonly IUserContext userContext;

    public ProcessReceiptHandler(IDbContext dbContext, IUserContext userContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<MediatR.Unit> Handle(ProcessReceipt request, CancellationToken cancellationToken)
    {
      Receipt receipt = await dbContext.Receipts
        .Include(x => x.Store).ThenInclude(x => x!.Banner)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Receipt>(request.Id);

      if (receipt.Store == null)
      {
        throw new InvalidOperationException($"The {nameof(receipt.Store)} is required.");
      }

      Dictionary<int, Item> items = await dbContext.Items
        .Include(x => x.Product).ThenInclude(x => x!.Article)
        .Include(x => x.Product).ThenInclude(x => x!.Department)
        .Where(x => x.ReceiptId == receipt.Id)
        .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

      Dictionary<int, Line> lines = await dbContext.Lines
        .Where(x => x.ReceiptId == receipt.Id)
        .ToDictionaryAsync(x => x.ItemId, x => x, cancellationToken);

      foreach (var (category, itemIds) in request.Payload.Items)
      {
        foreach (int itemId in itemIds)
        {
          if (items.TryGetValue(itemId, out Item? item))
          {
            if (item.Product == null)
            {
              throw new InvalidOperationException("The Product is required.");
            }
            else if (item.Product.Article == null)
            {
              throw new InvalidOperationException("The Article is required.");
            }

            if (lines.TryGetValue(item.Id, out Line? line))
            {
              line.Update(userContext.Id);
            }
            else
            {
              line = new Line(userContext.Id)
              {
                ArticleId = item.Product.Article.Id,
                ItemId = item.Id,
                ProductId = item.Product.Id,
                ReceiptId = receipt.Id,
                StoreId = receipt.Store.Id
              };
              lines.Add(item.Id, line);
              dbContext.Lines.Add(line);
            }

            line.ArticleName = item.Product.Article.Name;
            line.BannerId = receipt.Store.Banner?.Id;
            line.BannerName = receipt.Store.Banner?.Name;
            line.Category = category;
            line.DepartmentId = item.Product.Department?.Id;
            line.DepartmentName = item.Product.Department?.Name;
            line.DepartmentNumber = item.Product.Department?.Number;
            line.Flags = item.Product.Flags;
            line.Gtin = item.Product.Article.Gtin;
            line.IssuedAt = receipt.IssuedAt;
            line.Price = item.Price;
            line.ProductLabel = item.Product.Label;
            line.Quantity = item.Quantity ?? 1;
            line.ReceiptNumber = receipt.Number;
            line.Sku = item.Product.Sku;
            line.StoreName = receipt.Store.Name;
            line.StoreNumber = receipt.Store.Number;
            line.UnitPrice = item.UnitPrice;
            line.UnitType = item.Product.UnitType;
          }
        }
      }

      await dbContext.SaveChangesAsync(cancellationToken);

      return MediatR.Unit.Value;
    }
  }
}
