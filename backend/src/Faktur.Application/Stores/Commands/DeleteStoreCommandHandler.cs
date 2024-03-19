using Faktur.Application.Products.Commands;
using Faktur.Application.Receipts.Commands;
using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using MediatR;

namespace Faktur.Application.Stores.Commands;

internal class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, Store?>
{
  private readonly IPublisher _publisher;
  private readonly IStoreQuerier _storeQuerier;
  private readonly IStoreRepository _storeRepository;

  public DeleteStoreCommandHandler(IPublisher publisher, IStoreQuerier storeQuerier, IStoreRepository storeRepository)
  {
    _publisher = publisher;
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

    await _publisher.Publish(new DeleteStoreProductsCommand(command.ActorId, store), cancellationToken);
    await _publisher.Publish(new DeleteStoreReceiptsCommand(command.ActorId, store), cancellationToken);
    await _storeRepository.SaveAsync(store, cancellationToken);

    return result;
  }
}
