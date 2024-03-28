using Faktur.Application.Banners;
using Faktur.Application.Stores.Validators;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Faktur.Application.Stores.Commands;

internal class ReplaceStoreCommandHandler : IRequestHandler<ReplaceStoreCommand, Store?>
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreQuerier _storeQuerier;
  private readonly IStoreRepository _storeRepository;

  public ReplaceStoreCommandHandler(IBannerRepository bannerRepository, IStoreQuerier storeQuerier, IStoreRepository storeRepository)
  {
    _bannerRepository = bannerRepository;
    _storeQuerier = storeQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Store?> Handle(ReplaceStoreCommand command, CancellationToken cancellationToken)
  {
    ReplaceStorePayload payload = command.Payload;
    new ReplaceStoreValidator().ValidateAndThrow(payload);

    StoreAggregate? store = await _storeRepository.LoadAsync(command.Id, cancellationToken);
    if (store == null)
    {
      return null;
    }
    StoreAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _storeRepository.LoadAsync(command.Id, command.Version.Value, cancellationToken);
    }

    BannerAggregate? banner = null;
    if (payload.BannerId.HasValue)
    {
      banner = await _bannerRepository.LoadAsync(payload.BannerId.Value, cancellationToken)
        ?? throw new BannerNotFoundException(payload.BannerId.Value, nameof(payload.BannerId));
    }

    if (reference == null || banner?.Id != reference.BannerId)
    {
      store.BannerId = banner?.Id;
    }
    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    if (reference == null || number != reference.Number)
    {
      store.Number = number;
    }
    DisplayNameUnit displayName = new(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      store.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      store.Description = description;
    }

    AddressUnit? address = payload.Address?.ToAddressUnit(payload.Address.IsVerified);
    if (reference == null || address != reference.Address)
    {
      store.Address = address;
    }
    EmailUnit? email = payload.Email?.ToEmailUnit(payload.Email.IsVerified);
    if (reference == null || email != reference.Email)
    {
      store.Email = email;
    }
    PhoneUnit? phone = payload.Phone?.ToPhoneUnit(payload.Phone.IsVerified);
    if (reference == null || phone != reference.Phone)
    {
      store.Phone = phone;
    }

    store.Update(command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return await _storeQuerier.ReadAsync(store, cancellationToken);
  }
}
