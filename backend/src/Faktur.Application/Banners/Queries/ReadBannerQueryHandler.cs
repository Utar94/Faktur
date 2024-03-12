using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Queries;

internal class ReadBannerQueryHandler : IRequestHandler<ReadBannerQuery, Banner?>
{
  private readonly IBannerQuerier _bannerQuerier;

  public ReadBannerQueryHandler(IBannerQuerier bannerQuerier)
  {
    _bannerQuerier = bannerQuerier;
  }

  public async Task<Banner?> Handle(ReadBannerQuery query, CancellationToken cancellationToken)
  {
    return await _bannerQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
