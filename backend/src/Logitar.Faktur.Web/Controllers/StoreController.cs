using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Web.Extensions;
using Logitar.Faktur.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Controllers;

[ApiController]
[Route("stores")]
public class StoreController : ControllerBase
{
  private readonly IStoreService storeService;

  public StoreController(IStoreService storeService)
  {
    this.storeService = storeService;
  }

  [HttpPost]
  public async Task<ActionResult<AcceptedCommand>> CreateAsync([FromBody] CreateStorePayload payload, CancellationToken cancellationToken)
  {
    AcceptedCommand result = await storeService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.GetBaseUrl()}/stores/{result.AggregateId}");

    return Accepted(uri, result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<AcceptedCommand>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return Accepted(await storeService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Store>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Store? store = await storeService.ReadAsync(id, cancellationToken);
    return store == null ? NotFound() : Ok(store);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AcceptedCommand>> ReplaceAsync(string id, [FromBody] ReplaceStorePayload payload, long? version, CancellationToken cancellationToken)
  {
    return Accepted(await storeService.ReplaceAsync(id, payload, version, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Store>>> SearchAsync([FromQuery] SearchStoresQuery query, CancellationToken cancellationToken)
  {
    return Ok(await storeService.SearchAsync(query.ToPayload(), cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AcceptedCommand>> UpdateAsync(string id, [FromBody] UpdateStorePayload payload, CancellationToken cancellationToken)
  {
    return Accepted(await storeService.UpdateAsync(id, payload, cancellationToken));
  }
}
