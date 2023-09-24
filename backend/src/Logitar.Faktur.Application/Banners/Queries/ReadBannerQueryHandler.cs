using Logitar.Faktur.Contracts.Banners;
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
    return await bannerQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
