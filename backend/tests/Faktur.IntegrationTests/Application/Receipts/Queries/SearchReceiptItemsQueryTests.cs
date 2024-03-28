using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchReceiptItemsQueryTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public SearchReceiptItemsQueryTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchReceiptItemsPayload payload = new();
    SearchReceiptItemsQuery query = new(payload);
    SearchResults<ReceiptItem> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store, actorId: ActorId);
    receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null), ActorId);
    receipt.SetItem(2, new ReceiptItemUnit(new GtinUnit("4011"), sku: null, new DisplayNameUnit("BANANES"), new FlagsUnit("MRJ"),
      quantity: 1.150d, unitPrice: 1.52m, price: 1.75m, departmentNumber: null, department: null));
    receipt.SetItem(3, new ReceiptItemUnit(new GtinUnit("4068"), sku: null, new DisplayNameUnit("OIGNON VERT"), new FlagsUnit("MRJ"),
      quantity: 1.0d, unitPrice: 1.49m, price: 1.49m, departmentNumber: null, department: null));

    ReceiptAggregate otherReceipt = new(store, actorId: ActorId);
    otherReceipt.SetItem(2, new ReceiptItemUnit(new GtinUnit("4011"), sku: null, new DisplayNameUnit("BANANES"), new FlagsUnit("MRJ"),
      quantity: 1.150d, unitPrice: 1.52m, price: 1.75m, departmentNumber: null, department: null));

    await _receiptRepository.SaveAsync([receipt, otherReceipt]);

    SearchReceiptItemsPayload payload = new(receipt.Id.ToGuid())
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("40__"));
    payload.Sort.Add(new ReceiptItemSortOption(ReceiptItemSort.Number));

    SearchReceiptItemsQuery query = new(payload);
    SearchResults<ReceiptItem> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    ReceiptItem result = Assert.Single(results.Items);
    Assert.Equal(3, result.Number);
  }
}
