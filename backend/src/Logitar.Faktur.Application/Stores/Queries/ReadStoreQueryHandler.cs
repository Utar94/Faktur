using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Queries;

internal class ReadStoreQueryHandler : IRequestHandler<ReadStoreQuery, Store?>
{
  private readonly IStoreQuerier storeQuerier;

  public ReadStoreQueryHandler(IStoreQuerier storeQuerier)
  {
    this.storeQuerier = storeQuerier;
  }

  public async Task<Store?> Handle(ReadStoreQuery query, CancellationToken cancellationToken)
  {
    return await storeQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
