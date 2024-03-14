using Faktur.Application.Products.Commands;
using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using MediatR;

namespace Faktur.Application.Departments.Commands;

internal class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Department?>
{
  private readonly IDepartmentQuerier _departmentQuerier;
  private readonly IPublisher _publisher;
  private readonly IStoreRepository _storeRepository;

  public DeleteDepartmentCommandHandler(IDepartmentQuerier departmentQuerier, IPublisher publisher, IStoreRepository storeRepository)
  {
    _departmentQuerier = departmentQuerier;
    _publisher = publisher;
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

    await _publisher.Publish(new RemoveProductDepartmentCommand(command.ActorId, store, number), cancellationToken);
    await _storeRepository.SaveAsync(store, cancellationToken);

    return result;
  }
}
