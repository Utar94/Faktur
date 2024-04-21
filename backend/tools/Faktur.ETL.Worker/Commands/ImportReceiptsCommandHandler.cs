using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportReceiptsCommandHandler : IRequestHandler<ImportReceiptsCommand, int>
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly ITaxRepository _taxRepository;

  public ImportReceiptsCommandHandler(IReceiptRepository receiptRepository, ITaxRepository taxRepository)
  {
    _receiptRepository = receiptRepository;
    _taxRepository = taxRepository;
  }

  public async Task<int> Handle(ImportReceiptsCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);

    Dictionary<Guid, ReceiptAggregate> receipts = (await _receiptRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Id.ToGuid(), x => x);
    int count = 0;
    foreach (Receipt receipt in command.Receipts)
    {
      ReceiptId id = new(receipt.Id);

      NumberUnit? number = NumberUnit.TryCreate(receipt.Number);
      if (receipts.TryGetValue(receipt.Id, out ReceiptAggregate? existingReceipt))
      {
        existingReceipt.IssuedOn = receipt.IssuedOn;
        existingReceipt.Number = number;

        ActorId updatedBy = new(receipt.UpdatedBy.Id);
        existingReceipt.Update(updatedBy);
      }
      else
      {
        StoreId storeId = new(receipt.Store.Id);
        StoreAggregate store = new(storeId.AggregateId);

        List<ReceiptItemUnit> items = new(capacity: receipt.Items.Count);
        Dictionary<ushort, CategoryUnit?> itemCategories = new(capacity: receipt.Items.Count);
        foreach (ReceiptItem item in receipt.Items)
        {
          NumberUnit? departmentNumber = null;
          DepartmentUnit? department = null;
          if (item.Department != null)
          {
            departmentNumber = new NumberUnit(item.Department.Number);
            department = new DepartmentUnit(new DisplayNameUnit(item.Department.DisplayName), DescriptionUnit.TryCreate(item.Department.Description));
          }

          items.Add(new ReceiptItemUnit(GtinUnit.TryCreate(item.Gtin), SkuUnit.TryCreate(item.Sku), new DisplayNameUnit(item.Label), FlagsUnit.TryCreate(item.Flags),
            item.Quantity, item.UnitPrice, item.Price, departmentNumber, department));
          itemCategories[item.Number] = CategoryUnit.TryCreate(item.Category);
        }

        ActorId createdBy = new(receipt.CreatedBy.Id);
        existingReceipt = ReceiptAggregate.Import(store, receipt.IssuedOn, number, items, taxes, createdBy, id);
        receipts[receipt.Id] = existingReceipt;

        if (receipt.HasBeenProcessed)
        {
          ActorId processedBy = receipt.ProcessedBy == null ? createdBy : new(receipt.ProcessedBy.Id);
          existingReceipt.Categorize(itemCategories, processedBy);
        }
      }

      if (existingReceipt.HasChanges)
      {
        existingReceipt.SetDates(receipt);
        count++;
      }
    }

    await _receiptRepository.SaveAsync(receipts.Values, cancellationToken);

    return count;
  }
}
