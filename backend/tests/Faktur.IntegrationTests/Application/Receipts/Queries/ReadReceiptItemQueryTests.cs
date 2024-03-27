using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadReceiptItemQueryTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public ReadReceiptItemQueryTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return null when the receipt item cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_item_cannot_be_found()
  {
    ReadReceiptItemQuery query = new(ReceiptId: Guid.NewGuid(), ItemNumber: 0);
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the receipt item when it is found.")]
  public async Task It_should_return_the_receipt_item_when_it_is_found()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store, actorId: ActorId);
    ushort number = 1;
    receipt.SetItem(number, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null), ActorId);
    await _receiptRepository.SaveAsync(receipt);

    ReadReceiptItemQuery query = new(receipt.Id.ToGuid(), number);
    ReceiptItem? item = await Mediator.Send(query);
    Assert.NotNull(item);
    Assert.Equal(receipt.Id.ToGuid(), item.Receipt.Id);
    Assert.Equal(number, item.Number);
  }
}
