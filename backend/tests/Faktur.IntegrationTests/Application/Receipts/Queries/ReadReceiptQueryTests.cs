using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadReceiptQueryTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public ReadReceiptQueryTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    ReadReceiptQuery query = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the receipt when it is found.")]
  public async Task It_should_return_the_receipt_when_it_is_found()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store);
    await _receiptRepository.SaveAsync(receipt);

    ReadReceiptQuery query = new(receipt.Id.ToGuid());
    Receipt? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(receipt.Id.ToGuid(), result.Id);
  }
}
