using Faktur.Application.Departments.Validators;
using Faktur.Contracts.Departments;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Departments.Commands;

internal class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Department?>
{
  private readonly IDepartmentQuerier _departmentQuerier;
  private readonly IStoreRepository _storeRepository;

  public UpdateDepartmentCommandHandler(IDepartmentQuerier departmentQuerier, IStoreRepository storeRepository)
  {
    _departmentQuerier = departmentQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Department?> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
  {
    NumberUnit number = new(command.Number, nameof(command.Number));

    UpdateDepartmentPayload payload = command.Payload;
    new UpdateDepartmentValidator().ValidateAndThrow(payload);

    StoreAggregate? store = await _storeRepository.LoadAsync(command.StoreId, cancellationToken);
    DepartmentUnit? department = store?.TryFindDepartment(number);
    if (store == null || department == null)
    {
      return null;
    }

    DisplayNameUnit displayName = DisplayNameUnit.TryCreate(payload.DisplayName) ?? department.DisplayName;

    DescriptionUnit? description = department.Description;
    if (payload.Description != null)
    {
      description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    department = new(displayName, description);
    store.SetDepartment(number, department, command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return await _departmentQuerier.ReadAsync(store, number, cancellationToken);
  }
}
