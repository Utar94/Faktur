using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Queries;

internal class ReadBannerQueryHandler : IRequestHandler<ReadBannerQuery, Banner?>
{
  private readonly IBannerQuerier bannerQuerier;

  public ReadBannerQueryHandler(IBannerQuerier bannerQuerier)
  {
    this.bannerQuerier = bannerQuerier;
  }

  public async Task<Banner?> Handle(ReadBannerQuery query, CancellationToken cancellationToken)
  {
    BannerId bannerId = BannerId.Parse(query.Id, nameof(query.Id));

    return await bannerQuerier.ReadAsync(bannerId, cancellationToken);
  }
}
