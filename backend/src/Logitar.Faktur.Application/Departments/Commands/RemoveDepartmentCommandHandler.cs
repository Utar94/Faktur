using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Commands;

internal class RemoveDepartmentCommandHandler : IRequestHandler<RemoveDepartmentCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public RemoveDepartmentCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(RemoveDepartmentCommand command, CancellationToken cancellationToken)
  {
    DepartmentNumberUnit number = DepartmentNumberUnit.Parse(command.Number, nameof(command.Number));

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    if (!store.RemoveDepartment(number, applicationContext.ActorId))
    {
      throw new DepartmentNotFoundException(store, number, nameof(command.Number));
    }

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
