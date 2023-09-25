using FluentValidation;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Application.Stores.Validators;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Commands;

internal class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public UpdateStoreCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateStoreCommand command, CancellationToken cancellationToken)
  {
    UpdateStorePayload payload = command.Payload;
    new UpdateStorePayloadValidator().ValidateAndThrow(payload);

    StoreId id = StoreId.Parse(command.Id, nameof(command.Id));
    StoreAggregate store = await storeRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(id.AggregateId, nameof(command.Id));

    if (payload.Number != null)
    {
      store.Number = StoreNumberUnit.TryCreate(payload.Number.Value);
    }
    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      store.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      store.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    if (payload.Phone != null)
    {
      store.Phone = payload.Phone.Value?.ToPhoneUnit();
    }

    store.Update(applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
