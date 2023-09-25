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

internal class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public UpdateDepartmentCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
  {
    DepartmentNumberUnit number = DepartmentNumberUnit.Parse(command.Number, nameof(command.Number));
    UpdateDepartmentPayload payload = command.Payload;
    new UpdateDepartmentPayloadValidator().ValidateAndThrow(payload);

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    if (!store.Departments.TryGetValue(number.Value, out DepartmentUnit? department))
    {
      throw new DepartmentNotFoundException(store, number, nameof(command.Number));
    }

    DisplayNameUnit displayName = DisplayNameUnit.TryCreate(payload.DisplayName) ?? department.DisplayName;
    DescriptionUnit? description = payload.Description == null ? department.Description : DescriptionUnit.TryCreate(payload.Description.Value);
    department = new(number, displayName, description);
    store.SetDepartment(department, applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
