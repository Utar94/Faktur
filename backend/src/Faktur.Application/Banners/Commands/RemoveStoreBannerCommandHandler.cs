using Faktur.Domain.Stores;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class RemoveStoreBannerCommandHandler : INotificationHandler<RemoveBannerCommand>
{
  private readonly IStoreRepository _storeRepository;

  public RemoveStoreBannerCommandHandler(IStoreRepository storeRepository)
  {
    _storeRepository = storeRepository;
  }

  public async Task Handle(RemoveBannerCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<StoreAggregate> stores = await _storeRepository.LoadAsync(command.Banner, cancellationToken);
    foreach (StoreAggregate store in stores)
    {
      store.BannerId = null;
      store.Update(command.ActorId);
    }
    await _storeRepository.SaveAsync(stores, cancellationToken);
  }
}
