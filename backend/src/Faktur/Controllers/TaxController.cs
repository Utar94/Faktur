using Faktur.Application.Taxes.Commands;
using Faktur.Application.Taxes.Queries;
using Faktur.Contracts.Taxes;
using Faktur.Extensions;
using Faktur.Models.Taxes;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("taxes")]
public class TaxController : ControllerBase
{
  private readonly IMediator _mediator;

  public TaxController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<Tax>> CreateAsync([FromBody] CreateTaxPayload payload, CancellationToken cancellationToken)
  {
    Tax tax = await _mediator.Send(new CreateTaxCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("taxes/{id}", new Dictionary<string, string> { ["id"] = tax.Id.ToString() });
    return Created(location, tax);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Tax>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Tax? tax = await _mediator.Send(new DeleteTaxCommand(id), cancellationToken);
    return tax == null ? NotFound() : Ok(tax);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Tax>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Tax? tax = await _mediator.Send(new ReadTaxQuery(id, Code: null), cancellationToken);
    return tax == null ? NotFound() : Ok(tax);
  }

  [HttpGet("code:{code}")]
  public async Task<ActionResult<Tax>> ReadAsync(string code, CancellationToken cancellationToken)
  {
    Tax? tax = await _mediator.Send(new ReadTaxQuery(Id: null, code), cancellationToken);
    return tax == null ? NotFound() : Ok(tax);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Tax>> ReplaceAsync(Guid id, [FromBody] ReplaceTaxPayload payload, long? version, CancellationToken cancellationToken)
  {
    Tax? tax = await _mediator.Send(new ReplaceTaxCommand(id, payload, version), cancellationToken);
    return tax == null ? NotFound() : Ok(tax);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Tax>>> SearchAsync([FromQuery] SearchTaxesModel model, CancellationToken cancellationToken)
  {
    SearchResults<Tax> results = await _mediator.Send(new SearchTaxesQuery(model.ToPayload()), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Tax>> UpdateAsync(Guid id, [FromBody] UpdateTaxPayload payload, CancellationToken cancellationToken)
  {
    Tax? tax = await _mediator.Send(new UpdateTaxCommand(id, payload), cancellationToken);
    return tax == null ? NotFound() : Ok(tax);
  }
}
