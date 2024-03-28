using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchBannersQueryTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public SearchBannersQueryTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchBannersPayload payload = new();
    SearchBannersQuery query = new(payload);
    SearchResults<Banner> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    BannerAggregate notInIds = new(new DisplayNameUnit("MAXI&CIE"));
    BannerAggregate notMatching = new(new DisplayNameUnit("METRO+"));
    BannerAggregate banner = new(new DisplayNameUnit("IGA"));
    BannerAggregate expected = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync([notInIds, notMatching, banner, expected]);

    SearchBannersPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("MAXI%"));
    payload.Search.Terms.Add(new SearchTerm("IGA"));
    payload.Sort.Add(new BannerSortOption(BannerSort.DisplayName));

    IEnumerable<BannerAggregate> allBanners = await _bannerRepository.LoadAsync();
    payload.Ids.AddRange(allBanners.Select(banner => banner.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchBannersQuery query = new(payload);
    SearchResults<Banner> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Banner result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
