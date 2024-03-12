using Faktur.Contracts.Banners;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Banners.Queries;

internal class SearchBannersQueryHandler : IRequestHandler<SearchBannersQuery, SearchResults<Banner>>
{
  private readonly IBannerQuerier _bannerQuerier;

  public SearchBannersQueryHandler(IBannerQuerier bannerQuerier)
  {
    _bannerQuerier = bannerQuerier;
  }

  public async Task<SearchResults<Banner>> Handle(SearchBannersQuery query, CancellationToken cancellationToken)
  {
    return await _bannerQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
