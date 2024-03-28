using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchStoresQueryTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  public SearchStoresQueryTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchStoresPayload payload = new();
    SearchStoresQuery query = new(payload);
    SearchResults<Store> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    BannerAggregate otherBanner = new(new DisplayNameUnit("IGA"));
    await _bannerRepository.SaveAsync([banner, otherBanner]);

    StoreAggregate otherBannerStore = new(new DisplayNameUnit("IGA Drummondville"))
    {
      BannerId = otherBanner.Id
    };
    otherBanner.Update();
    StoreAggregate notInIds = new(new DisplayNameUnit("Maxi Sherbrooke"))
    {
      BannerId = banner.Id,
      Number = new NumberUnit("08851")
    };
    notInIds.Update();
    StoreAggregate notMatching = new(new DisplayNameUnit("Maxi Saint-Hyacinthe"))
    {
      BannerId = banner.Id
    };
    notMatching.Update();
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"))
    {
      BannerId = banner.Id
    };
    store.Update();
    StoreAggregate expected = new(new DisplayNameUnit("Maxi Saint-Charles-de-Drummondville"))
    {
      BannerId = banner.Id
    };
    expected.Update();
    await _storeRepository.SaveAsync([notInIds, notMatching, store, expected]);

    SearchStoresPayload payload = new()
    {
      BannerId = banner.Id.ToGuid(),
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%Drummondville"));
    payload.Search.Terms.Add(new SearchTerm("_88__"));
    payload.Sort.Add(new StoreSortOption(StoreSort.DisplayName));

    IEnumerable<StoreAggregate> allStores = await _storeRepository.LoadAsync();
    payload.Ids.AddRange(allStores.Select(store => store.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchStoresQuery query = new(payload);
    SearchResults<Store> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Store result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
