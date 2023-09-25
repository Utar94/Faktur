using Logitar.Faktur.Application.Stores.Commands;
using Logitar.Faktur.Application.Stores.Queries;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores;

internal class StoreService : IStoreService
{
  private readonly IMediator mediator;

  public StoreService(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public async Task<AcceptedCommand> CreateAsync(CreateStorePayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new CreateStoreCommand(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new DeleteStoreCommand(id), cancellationToken);
  }

  public async Task<Store?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReadStoreQuery(id), cancellationToken);
  }

  public async Task<AcceptedCommand> ReplaceAsync(string id, ReplaceStorePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReplaceStoreCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SearchStoresQuery(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> UpdateAsync(string id, UpdateStorePayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new UpdateStoreCommand(id, payload), cancellationToken);
  }
}
