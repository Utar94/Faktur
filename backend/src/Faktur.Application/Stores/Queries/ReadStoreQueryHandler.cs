using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.Application.Stores.Queries;

internal class ReadStoreQueryHandler : IRequestHandler<ReadStoreQuery, Store?>
{
  private readonly IStoreQuerier _storeQuerier;

  public ReadStoreQueryHandler(IStoreQuerier storeQuerier)
  {
    _storeQuerier = storeQuerier;
  }

  public async Task<Store?> Handle(ReadStoreQuery query, CancellationToken cancellationToken)
  {
    return await _storeQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
