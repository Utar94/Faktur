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

  [HttpPost("import")]
  public async Task<ActionResult<Receipt>> ImportAsync([FromBody] ImportReceiptPayload payload, CancellationToken cancellationToken)
  {
    Receipt receipt = await _mediator.Send(new ImportReceiptCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("receipts/{id}", new Dictionary<string, string> { ["id"] = receipt.Id.ToString() });
    return Created(location, receipt);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Receipt>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Receipt? receipt = await _mediator.Send(new ReadReceiptQuery(id), cancellationToken);
    return receipt == null ? NotFound() : Ok(receipt);
  }
}
