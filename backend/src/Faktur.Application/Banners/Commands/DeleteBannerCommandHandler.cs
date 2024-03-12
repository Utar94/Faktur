using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Banner?>
{
  private readonly IBannerQuerier _bannerQuerier;
  private readonly IBannerRepository _bannerRepository;

  public DeleteBannerCommandHandler(IBannerQuerier bannerQuerier, IBannerRepository bannerRepository)
  {
    _bannerQuerier = bannerQuerier;
    _bannerRepository = bannerRepository;
  }

  public async Task<Banner?> Handle(DeleteBannerCommand command, CancellationToken cancellationToken)
  {
    BannerAggregate? banner = await _bannerRepository.LoadAsync(command.Id, cancellationToken);
    if (banner == null)
    {
      return null;
    }
    Banner result = await _bannerQuerier.ReadAsync(banner, cancellationToken);

    banner.Delete(command.ActorId);

    await _bannerRepository.SaveAsync(banner, cancellationToken);

    return result;
  }
}
