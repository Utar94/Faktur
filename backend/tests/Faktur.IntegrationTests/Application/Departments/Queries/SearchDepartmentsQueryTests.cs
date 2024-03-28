using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchDepartmentsQueryTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  public SearchDepartmentsQueryTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchDepartmentsPayload payload = new();
    SearchDepartmentsQuery query = new(payload);
    SearchResults<Department> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    store.SetDepartment(new NumberUnit("22"), new DepartmentUnit(new DisplayNameUnit("PRODUITS LAITIERS")));
    store.SetDepartment(new NumberUnit("27"), new DepartmentUnit(new DisplayNameUnit("FRUITS ET LEGUMES")));
    store.SetDepartment(new NumberUnit("31"), new DepartmentUnit(new DisplayNameUnit("VIANDE")));

    StoreAggregate otherStore = new(new DisplayNameUnit("Maxi Sherbrooke"));
    otherStore.SetDepartment(new NumberUnit("21"), new DepartmentUnit(new DisplayNameUnit("EPICERIE")));

    await _storeRepository.SaveAsync([store, otherStore]);

    SearchDepartmentsPayload payload = new(store.Id.ToGuid())
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("2_"));
    payload.Sort.Add(new DepartmentSortOption(DepartmentSort.DisplayName));

    SearchDepartmentsQuery query = new(payload);
    SearchResults<Department> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Department result = Assert.Single(results.Items);
    Assert.Equal("22", result.Number);
    Assert.Equal(store.Id.ToGuid(), result.Store.Id);
  }
}
