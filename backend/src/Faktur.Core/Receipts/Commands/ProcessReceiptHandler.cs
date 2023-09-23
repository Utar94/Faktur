using AutoMapper;
using Faktur.Core.Receipts.Models;
using Faktur.Core.Stores;
using Logitar.Identity.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Commands
{
  public class ProcessReceiptHandler : IRequestHandler<ProcessReceipt, ReceiptModel>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public ProcessReceiptHandler(IDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<ReceiptModel> Handle(ProcessReceipt request, CancellationToken cancellationToken)
    {
      Receipt receipt = await dbContext.Receipts
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Article)
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Department)
        .Include(x => x.Store)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Receipt>(request.Id);

      Store store = receipt.Store
        ?? throw new InvalidOperationException($"The {nameof(receipt.Store)} is required.");

      Dictionary<int, Item> items = receipt.Items.ToDictionary(x => x.Id, x => x);

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
                StoreId = store.Id
              };
              lines.Add(item.Id, line);
              dbContext.Lines.Add(line);
            }

            line.ArticleName = item.Product.Article.Name;
            line.BannerId = store.Banner?.Id;
            line.BannerName = store.Banner?.Name;
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
            line.StoreName = store.Name;
            line.StoreNumber = store.Number;
            line.UnitPrice = item.UnitPrice;
            line.UnitType = item.Product.UnitType;
          }
        }
      }

      receipt.Process(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ReceiptModel>(receipt);
    }
  }
}
