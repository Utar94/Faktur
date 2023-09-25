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

internal class ReplaceStoreCommandHandler : IRequestHandler<ReplaceStoreCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public ReplaceStoreCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(ReplaceStoreCommand command, CancellationToken cancellationToken)
  {
    ReplaceStorePayload payload = command.Payload;
    new ReplaceStorePayloadValidator().ValidateAndThrow(payload);

    StoreId id = StoreId.Parse(command.Id, nameof(command.Id));
    StoreAggregate store = await storeRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(id.AggregateId, nameof(command.Id));

    StoreAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await storeRepository.LoadAsync(store.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || (payload.Number?.CleanTrim() != reference.Number?.Value))
    {
      store.Number = StoreNumberUnit.TryCreate(payload.Number);
    }
    if (reference == null || (payload.DisplayName.Trim() != reference.DisplayName.Value))
    {
      store.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description?.Value))
    {
      store.Description = DescriptionUnit.TryCreate(payload.Description);
    }

    PhoneUnit? phone = payload.Phone?.ToPhoneUnit();
    if (reference == null || phone != reference.Phone)
    {
      store.Phone = phone;
    }

    store.Update(applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
