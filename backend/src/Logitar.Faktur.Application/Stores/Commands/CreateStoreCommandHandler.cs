using FluentValidation;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Application.Stores.Validators;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Commands;

internal class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;
  private readonly IStoreRepository storeRepository;

  public CreateStoreCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(CreateStoreCommand command, CancellationToken cancellationToken)
  {
    CreateStorePayload payload = command.Payload;
    new CreateStorePayloadValidator().ValidateAndThrow(payload);

    BannerAggregate? banner = null;
    if (!string.IsNullOrWhiteSpace(payload.BannerId))
    {
      BannerId bannerId = BannerId.Parse(payload.BannerId, nameof(payload.BannerId));
      banner = await bannerRepository.LoadAsync(bannerId, cancellationToken)
        ?? throw new AggregateNotFoundException<BannerAggregate>(bannerId.AggregateId, nameof(payload.BannerId));
    }

    StoreNumberUnit? number = StoreNumberUnit.TryCreate(payload.Number);

    StoreId? id = null;
    if (!string.IsNullOrWhiteSpace(payload.Id))
    {
      id = StoreId.Parse(payload.Id, nameof(payload.Id));
      if (await storeRepository.LoadAsync(id, cancellationToken) != null)
      {
        throw new IdentifierAlreadyUsedException<StoreAggregate>(id.AggregateId, nameof(payload.Id));
      }
    }
    else if (banner != null && number != null)
    {
      id = new(banner, number);
      if (await storeRepository.LoadAsync(id, cancellationToken) != null)
      {
        id = null;
      }
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    StoreAggregate store = new(displayName, applicationContext.ActorId, id)
    {
      Number = number,
      Description = DescriptionUnit.TryCreate(payload.Description),
      Address = payload.Address?.ToAddressUnit(),
      Phone = payload.Phone?.ToPhoneUnit()
    };
    store.SetBanner(banner);
    store.Update(applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
