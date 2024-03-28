using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class RemoveReceiptItemCommandTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _qst;
  private readonly StoreAggregate _store;
  private readonly ReceiptAggregate _receipt;

  public RemoveReceiptItemCommandTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05d, ActorId)
    {
      Flags = new FlagsUnit("F")
    };
    _gst.Update(ActorId);
    _qst = new(new TaxCodeUnit("QST"), rate: 0.09975d, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    _qst.Update(ActorId);
    _store = new StoreAggregate(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    _receipt = new(_store, actorId: ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync([_gst, _qst]);
    await _storeRepository.SaveAsync(_store);
    await _receiptRepository.SaveAsync(_receipt);
  }

  [Fact(DisplayName = "It should remove an existing receipt item.")]
  public async Task It_should_remove_an_existing_receipt_tem()
  {
    ushort number = 1;
    _receipt.SetItem(number, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), flags: null,
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null), ActorId);
    await _receiptRepository.SaveAsync(_receipt);

    RemoveReceiptItemCommand command = new(_receipt.Id.ToGuid(), number);
    ReceiptItem? item = await Mediator.Send(command);
    Assert.NotNull(item);
    Assert.Equal(_receipt.Id.ToGuid(), item.Receipt.Id);
    Assert.Equal(number, item.Number);

    ReceiptEntity? receipt = await FakturContext.Receipts.AsNoTracking()
      .Include(x => x.Items)
      .Include(x => x.Store)
      .Include(x => x.Taxes)
      .SingleOrDefaultAsync(x => x.AggregateId == _receipt.Id.Value);
    Assert.NotNull(receipt);
    Assert.Empty(receipt.Items);
    Assert.Empty(receipt.Taxes);
    Assert.Equal(0m, receipt.SubTotal);
    Assert.Equal(0m, receipt.Total);
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    RemoveReceiptItemCommand command = new(ReceiptId: Guid.NewGuid(), ItemNumber: 0);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should return null when the receipt item cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_item_cannot_be_found()
  {
    RemoveReceiptItemCommand command = new(_receipt.Id.ToGuid(), ItemNumber: 0);
    Assert.Null(await Mediator.Send(command));
  }
}
