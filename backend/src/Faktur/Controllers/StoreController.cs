using Faktur.Application.Stores.Commands;
using Faktur.Application.Stores.Queries;
using Faktur.Contracts.Stores;
using Faktur.Extensions;
using Faktur.Models.Stores;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("stores")]
public class StoreController : ControllerBase
{
  private readonly IMediator _mediator;

  public StoreController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<Store>> CreateAsync([FromBody] CreateStorePayload payload, CancellationToken cancellationToken)
  {
    Store store = await _mediator.Send(new CreateStoreCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("stores/{id}", new Dictionary<string, string> { ["id"] = store.Id.ToString() });
    return Created(location, store);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Store>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Store? store = await _mediator.Send(new DeleteStoreCommand(id), cancellationToken);
    return store == null ? NotFound() : Ok(store);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Store>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Store? store = await _mediator.Send(new ReadStoreQuery(id), cancellationToken);
    return store == null ? NotFound() : Ok(store);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Store>> ReplaceAsync(Guid id, [FromBody] ReplaceStorePayload payload, long? version, CancellationToken cancellationToken)
  {
    Store? store = await _mediator.Send(new ReplaceStoreCommand(id, payload, version), cancellationToken);
    return store == null ? NotFound() : Ok(store);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Store>>> SearchAsync([FromQuery] SearchStoresModel model, CancellationToken cancellationToken)
  {
    SearchResults<Store> results = await _mediator.Send(new SearchStoresQuery(model.ToPayload()), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Store>> UpdateAsync(Guid id, [FromBody] UpdateStorePayload payload, CancellationToken cancellationToken)
  {
    Store? store = await _mediator.Send(new UpdateStoreCommand(id, payload), cancellationToken);
    return store == null ? NotFound() : Ok(store);
  }
}
