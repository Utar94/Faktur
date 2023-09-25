using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;
  private readonly IStoreRepository storeRepository;

  public DeleteBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(DeleteBannerCommand command, CancellationToken cancellationToken)
  {
    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate banner = await bannerRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    await RemoveFromStoresAsync(banner, cancellationToken);

    banner.Delete(applicationContext.ActorId);

    await bannerRepository.SaveAsync(banner, cancellationToken);

    return applicationContext.AcceptCommand(banner);
  }

  private async Task RemoveFromStoresAsync(BannerAggregate banner, CancellationToken cancellationToken)
  {
    IEnumerable<StoreAggregate> stores = await storeRepository.LoadAsync(banner, cancellationToken);
    foreach (StoreAggregate store in stores)
    {
      store.SetBanner(null);
      store.Update(applicationContext.ActorId);
    }
    await storeRepository.SaveAsync(stores, cancellationToken);
  }
}
