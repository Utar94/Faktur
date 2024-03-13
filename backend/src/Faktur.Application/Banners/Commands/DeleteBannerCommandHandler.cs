using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Banner?>
{
  private readonly IBannerQuerier _bannerQuerier;
  private readonly IBannerRepository _bannerRepository;
  private readonly IPublisher _publisher;

  public DeleteBannerCommandHandler(IBannerQuerier bannerQuerier, IBannerRepository bannerRepository, IPublisher publisher)
  {
    _bannerQuerier = bannerQuerier;
    _bannerRepository = bannerRepository;
    _publisher = publisher;
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

    await _publisher.Publish(new RemoveBannerCommand(command.ActorId, banner), cancellationToken);
    await _bannerRepository.SaveAsync(banner, cancellationToken);

    return result;
  }
}
