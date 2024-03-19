using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchReceiptsQueryTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public SearchReceiptsQueryTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Receipts.Table, FakturDb.Stores.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchReceiptsPayload payload = new();
    SearchReceiptsQuery query = new(payload);
    SearchResults<Receipt> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    StoreAggregate otherStore = new(new DisplayNameUnit("Maxi Saint-Hyacinthe"));
    await _storeRepository.SaveAsync([store, otherStore]);

    NumberUnit receiptNumber = new("117011");
    ReceiptAggregate otherStoreReceipt = new(otherStore, number: receiptNumber);
    //ReceiptAggregate notEmpty = new(store, number: new NumberUnit("117012")); // TODO(fpion): set item
    //ReceiptAggregate processed = new(store, number: new NumberUnit("118945")); // TODO(fpion): process
    ReceiptAggregate notInIds = new(store, number: new NumberUnit("118899"));
    ReceiptAggregate notMatching = new(store);
    ReceiptAggregate receipt = new(store, DateTime.Now.AddHours(-1), new NumberUnit("117014"));
    ReceiptAggregate expected = new(store, DateTime.Now.AddDays(-1), receiptNumber);
    await _receiptRepository.SaveAsync([otherStoreReceipt, notInIds, notMatching, receipt, expected]);

    SearchReceiptsPayload payload = new()
    {
      StoreId = store.Id.ToGuid(),
      IsEmpty = true,
      HasBeenProcessed = false,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("117%"));
    payload.Search.Terms.Add(new SearchTerm("118%"));
    payload.Sort.Add(new ReceiptSortOption(ReceiptSort.IssuedOn, isDescending: true));

    IEnumerable<ReceiptAggregate> allReceipts = await _receiptRepository.LoadAsync();
    payload.Ids.AddRange(allReceipts.Select(receipt => receipt.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchReceiptsQuery query = new(payload);
    SearchResults<Receipt> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Receipt result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
