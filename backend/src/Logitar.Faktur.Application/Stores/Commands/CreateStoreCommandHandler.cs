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

internal class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public CreateStoreCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(CreateStoreCommand command, CancellationToken cancellationToken)
  {
    CreateStorePayload payload = command.Payload;
    new CreateStorePayloadValidator().ValidateAndThrow(payload);

    StoreId? id = null;
    if (!string.IsNullOrWhiteSpace(payload.Id))
    {
      id = StoreId.Parse(payload.Id, nameof(payload.Id));
      if (await storeRepository.LoadAsync(id, cancellationToken) != null)
      {
        throw new IdentifierAlreadyUsedException<StoreAggregate>(id.AggregateId, nameof(payload.Id));
      }
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    StoreAggregate store = new(displayName, applicationContext.ActorId, id)
    {
      Number = StoreNumberUnit.TryCreate(payload.Number),
      Description = DescriptionUnit.TryCreate(payload.Description),
      Address = payload.Address?.ToAddressUnit(),
      Phone = payload.Phone?.ToPhoneUnit()
    };
    store.Update(applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
