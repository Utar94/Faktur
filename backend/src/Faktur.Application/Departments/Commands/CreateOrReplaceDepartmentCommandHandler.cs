using Faktur.Application.Departments.Validators;
using Faktur.Contracts.Departments;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Departments.Commands;

internal class CreateOrReplaceDepartmentCommandHandler : IRequestHandler<CreateOrReplaceDepartmentCommand, CreateOrReplaceDepartmentResult?>
{
  private readonly IDepartmentQuerier _departmentQuerier;
  private readonly IStoreRepository _storeRepository;

  public CreateOrReplaceDepartmentCommandHandler(IDepartmentQuerier departmentQuerier, IStoreRepository storeRepository)
  {
    _departmentQuerier = departmentQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<CreateOrReplaceDepartmentResult?> Handle(CreateOrReplaceDepartmentCommand command, CancellationToken cancellationToken)
  {
    NumberUnit number = new(command.Number, nameof(command.Number));

    CreateOrReplaceDepartmentPayload payload = command.Payload;
    new CreateOrReplaceDepartmentValidator().ValidateAndThrow(payload);

    DisplayNameUnit displayName = new(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);

    StoreAggregate? store = await _storeRepository.LoadAsync(command.StoreId, cancellationToken);
    if (store == null)
    {
      return null;
    }
    DepartmentUnit? reference = null;
    if (command.Version.HasValue)
    {
      StoreAggregate? storeReference = await _storeRepository.LoadAsync(command.StoreId, command.Version.Value, cancellationToken);
      if (storeReference != null)
      {
        reference = storeReference.TryFindDepartment(number);
      }
    }

    DepartmentUnit? department = store.TryFindDepartment(number);
    bool isCreated = department == null;
    if (department == null || reference == null)
    {
      department = new(displayName, description);
      store.SetDepartment(number, department, command.ActorId);
    }
    else
    {
      if (displayName == reference.DisplayName)
      {
        displayName = department.DisplayName;
      }
      if (description == reference.Description)
      {
        description = department.Description;
      }

      department = new(displayName, description);
      store.SetDepartment(number, department, command.ActorId);
    }

    await _storeRepository.SaveAsync(store, cancellationToken);

    Department result = await _departmentQuerier.ReadAsync(store, number, cancellationToken);
    return new CreateOrReplaceDepartmentResult(isCreated, result);
  }
}
