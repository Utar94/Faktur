using FluentValidation;
using Logitar.Faktur.Application.Departments.Validators;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Commands;

internal class SaveDepartmentCommandHandler : IRequestHandler<SaveDepartmentCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public SaveDepartmentCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(SaveDepartmentCommand command, CancellationToken cancellationToken)
  {
    DepartmentNumberUnit number = DepartmentNumberUnit.Parse(command.Number, nameof(command.Number));
    SaveDepartmentPayload payload = command.Payload;
    new SaveDepartmentPayloadValidator().ValidateAndThrow(payload);

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    DisplayNameUnit displayName = new(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    DepartmentUnit department = new(number, displayName, description);
    store.SetDepartment(department, applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
