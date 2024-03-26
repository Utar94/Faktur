using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Taxes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchTaxesQueryTests : IntegrationTests
{
  private readonly ITaxRepository _taxRepository;

  public SearchTaxesQueryTests() : base()
  {
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchTaxesPayload payload = new();
    SearchTaxesQuery query = new(payload);
    SearchResults<Tax> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    TaxAggregate notInIds = new(new TaxCodeUnit("HST"), rate: 0.13, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    notInIds.Update(ActorId);

    TaxAggregate notMatching = new(new TaxCodeUnit("TEST"), rate: 0.025, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    notMatching.Update(ActorId);

    TaxAggregate notFlag = new(new TaxCodeUnit("GST"), rate: 0.05, ActorId)
    {
      Flags = new FlagsUnit("F")
    };
    notFlag.Update(ActorId);

    TaxAggregate tax = new(new TaxCodeUnit("EST"), rate: 0.15, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    tax.Update(ActorId);

    TaxAggregate expected = new(new TaxCodeUnit("QST"), rate: 0.09975, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    expected.Update(ActorId);

    await _taxRepository.SaveAsync([notInIds, notMatching, notFlag, tax, expected]);

    SearchTaxesPayload payload = new()
    {
      Flag = 'P',
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("_ST"));
    payload.Sort.Add(new TaxSortOption(TaxSort.Rate, isDescending: true));

    IEnumerable<TaxAggregate> allTaxes = await _taxRepository.LoadAsync();
    payload.Ids.AddRange(allTaxes.Select(tax => tax.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchTaxesQuery query = new(payload);
    SearchResults<Tax> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Tax result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
