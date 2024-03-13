using Faktur.Application.Banners;
using Faktur.Application.Stores.Validators;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Stores.Commands;

internal class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, Store?>
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreQuerier _storeQuerier;
  private readonly IStoreRepository _storeRepository;

  public UpdateStoreCommandHandler(IBannerRepository bannerRepository, IStoreQuerier storeQuerier, IStoreRepository storeRepository)
  {
    _bannerRepository = bannerRepository;
    _storeQuerier = storeQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Store?> Handle(UpdateStoreCommand command, CancellationToken cancellationToken)
  {
    UpdateStorePayload payload = command.Payload;
    new UpdateStoreValidator().ValidateAndThrow(payload);

    StoreAggregate? store = await _storeRepository.LoadAsync(command.Id, cancellationToken);
    if (store == null)
    {
      return null;
    }

    if (payload.BannerId != null)
    {
      BannerAggregate? banner = null;
      if (payload.BannerId.Value.HasValue)
      {
        banner = await _bannerRepository.LoadAsync(payload.BannerId.Value.Value, cancellationToken)
          ?? throw new BannerNotFoundException(payload.BannerId.Value.Value, nameof(payload.BannerId));
      }
      store.BannerId = banner?.Id;
    }

    if (payload.Number != null)
    {
      store.Number = NumberUnit.TryCreate(payload.Number.Value);
    }
    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      store.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      store.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    store.Update(command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return await _storeQuerier.ReadAsync(store, cancellationToken);
  }
}
