using Faktur.Application.Banners;
using Faktur.Application.Stores.Validators;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Stores.Commands;

internal class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Store>
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreQuerier _storeQuerier;
  private readonly IStoreRepository _storeRepository;

  public CreateStoreCommandHandler(IBannerRepository bannerRepository, IStoreQuerier storeQuerier, IStoreRepository storeRepository)
  {
    _bannerRepository = bannerRepository;
    _storeQuerier = storeQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Store> Handle(CreateStoreCommand command, CancellationToken cancellationToken)
  {
    CreateStorePayload payload = command.Payload;
    new CreateStoreValidator().ValidateAndThrow(payload);

    BannerAggregate? banner = null;
    if (payload.BannerId.HasValue)
    {
      banner = await _bannerRepository.LoadAsync(payload.BannerId.Value, cancellationToken)
        ?? throw new BannerNotFoundException(payload.BannerId.Value, nameof(payload.BannerId));
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    StoreAggregate store = new(displayName, command.ActorId)
    {
      BannerId = banner?.Id,
      Number = NumberUnit.TryCreate(payload.Number),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };

    store.Update(command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return await _storeQuerier.ReadAsync(store, cancellationToken);
  }
}
