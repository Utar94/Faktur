using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using MediatR;

namespace Faktur.Application.Stores.Commands;

internal class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, Store?>
{
  private readonly IStoreQuerier _storeQuerier;
  private readonly IStoreRepository _storeRepository;

  public DeleteStoreCommandHandler(IStoreQuerier storeQuerier, IStoreRepository storeRepository)
  {
    _storeQuerier = storeQuerier;
    _storeRepository = storeRepository;
  }

  public async Task<Store?> Handle(DeleteStoreCommand command, CancellationToken cancellationToken)
  {
    StoreAggregate? store = await _storeRepository.LoadAsync(command.Id, cancellationToken);
    if (store == null)
    {
      return null;
    }
    Store result = await _storeQuerier.ReadAsync(store, cancellationToken);

    store.Delete(command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);

    return result;
  }
}
