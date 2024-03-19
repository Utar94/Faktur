using Faktur.Application.Receipts.Commands;
using Faktur.Application.Receipts.Queries;
using Faktur.Contracts.Receipts;
using Faktur.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("receipts")]
public class ReceiptController : ControllerBase
{
  private readonly IMediator _mediator;

  public ReceiptController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("/stores/{storeId}/receipts")]
  public async Task<ActionResult<Receipt>> CreateAsync(Guid storeId, [FromBody] CreateReceiptPayload payload, CancellationToken cancellationToken)
  {
    Receipt receipt = await _mediator.Send(new CreateReceiptCommand(storeId, payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("receipts/{id}", new Dictionary<string, string> { ["id"] = receipt.Id.ToString() });
    return Created(location, receipt);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Receipt>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Receipt? receipt = await _mediator.Send(new DeleteReceiptCommand(id), cancellationToken);
    return receipt == null ? NotFound() : Ok(receipt);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Receipt>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Receipt? receipt = await _mediator.Send(new ReadReceiptQuery(id), cancellationToken);
    return receipt == null ? NotFound() : Ok(receipt);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Receipt>> ReplaceAsync(Guid id, [FromBody] ReplaceReceiptPayload payload, long? version, CancellationToken cancellationToken)
  {
    Receipt? receipt = await _mediator.Send(new ReplaceReceiptCommand(id, payload, version), cancellationToken);
    return receipt == null ? NotFound() : Ok(receipt);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Receipt>> UpdateAsync(Guid id, [FromBody] UpdateReceiptPayload payload, CancellationToken cancellationToken)
  {
    Receipt? receipt = await _mediator.Send(new UpdateReceiptCommand(id, payload), cancellationToken);
    return receipt == null ? NotFound() : Ok(receipt);
  }
}
