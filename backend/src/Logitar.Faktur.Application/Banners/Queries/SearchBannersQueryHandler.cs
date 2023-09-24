using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Queries;

internal class SearchBannersQueryHandler : IRequestHandler<SearchBannersQuery, SearchResults<Banner>>
{
  private readonly IBannerQuerier bannerQuerier;

  public SearchBannersQueryHandler(IBannerQuerier bannerQuerier)
  {
    this.bannerQuerier = bannerQuerier;
  }

  public async Task<SearchResults<Banner>> Handle(SearchBannersQuery query, CancellationToken cancellationToken)
  {
    return await bannerQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
