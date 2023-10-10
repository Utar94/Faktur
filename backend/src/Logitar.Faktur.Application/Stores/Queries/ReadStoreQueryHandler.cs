using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;
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
    StoreId storeId = StoreId.Parse(query.Id, nameof(query.Id));

    return await storeQuerier.ReadAsync(storeId, cancellationToken);
  }
}
