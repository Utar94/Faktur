using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using MediatR;

namespace Faktur.Application.Departments.Commands;

internal class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Department?>
{
  private readonly IDepartmentQuerier _departmentQuerier;
  private readonly IStoreRepository _storeRepository;

  public DeleteDepartmentCommandHandler(IDepartmentQuerier departmentQuerier, IStoreRepository storeRepository)
  {
    _departmentQuerier = departmentQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Department?> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
  {
    NumberUnit number = new(command.Number, nameof(command.Number));

    StoreAggregate? store = await _storeRepository.LoadAsync(command.StoreId, cancellationToken);
    if (store == null || store.TryFindDepartment(number) == null)
    {
      return null;
    }
    Department result = await _departmentQuerier.ReadAsync(store, number, cancellationToken);

    store.RemoveDepartment(number, command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return result;
  }
}
