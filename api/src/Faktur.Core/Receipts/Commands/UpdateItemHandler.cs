using AutoMapper;
using Faktur.Core.Receipts.Models;
using Faktur.Core.Settings;
using Logitar.Identity.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Commands
{
  internal class UpdateItemHandler : IRequestHandler<UpdateItem, ReceiptModel>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;
    private readonly TaxesSettings taxesSettings;
    private readonly IUserContext userContext;

    public UpdateItemHandler(
      IDbContext dbContext,
      IMapper mapper,
      TaxesSettings taxesSettings,
      IUserContext userContext
    )
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this.taxesSettings = taxesSettings ?? throw new ArgumentNullException(nameof(taxesSettings));
      this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<ReceiptModel> Handle(UpdateItem request, CancellationToken cancellationToken)
    {
      Item item = await dbContext.Items
        .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Item>(request.Id);

      Receipt receipt = await dbContext.Receipts
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Article)
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Department)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.Id == item.ReceiptId, cancellationToken)
        ?? throw new EntityNotFoundException<Receipt>(request.Id);

      item.Quantity = request.Payload.Quantity ?? 1;
      item.UnitPrice = request.Payload.UnitPrice;

      item.Price = request.Payload.Price ?? (decimal)item.Quantity * item.UnitPrice;

      receipt.CalculateSubTotal();
      receipt.CalculateTax(taxesSettings.Gst);
      receipt.CalculateTax(taxesSettings.Qst);
      receipt.CalculateTotal();

      receipt.Update(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ReceiptModel>(receipt);
    }
  }
}
