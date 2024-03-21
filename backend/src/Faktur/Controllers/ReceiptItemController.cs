using Faktur.Application.Receipts.Commands;
using Faktur.Application.Receipts.Queries;
using Faktur.Contracts.Receipts;
using Faktur.Extensions;
using Faktur.Models.Receipts;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("receipts/{receiptId}/items")]
public class ReceiptItemController : ControllerBase
{
  private readonly IMediator _mediator;

  public ReceiptItemController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPut("{number}")]
  public async Task<ActionResult<ReceiptItem>> CreateOrReplaceAsync(Guid receiptId, ushort number, [FromBody] CreateOrReplaceReceiptItemPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceReceiptItemResult? result = await _mediator.Send(new CreateOrReplaceReceiptItemCommand(receiptId, number, payload, version), cancellationToken);
    if (result == null)
    {
      return NotFound();
    }
    else if (result.IsCreated)
    {
      ReceiptItem item = result.Item;
      Uri location = HttpContext.BuildLocation("receipts/{receiptId}/items/{number}", new Dictionary<string, string>
      {
        ["receiptId"] = receiptId.ToString(),
        ["number"] = item.Number.ToString()
      });
      return Created(location, item);
    }
    else
    {
      return Ok(result.Item);
    }
  }

  [HttpGet("{number}")]
  public async Task<ActionResult<ReceiptItem>> ReadAsync(Guid receiptId, ushort number, CancellationToken cancellationToken)
  {
    ReceiptItem? item = await _mediator.Send(new ReadReceiptItemQuery(receiptId, number), cancellationToken);
    return item == null ? NotFound() : Ok(item);
  }

  [HttpDelete("{number}")]
  public async Task<ActionResult<ReceiptItem>> RemoveAsync(Guid receiptId, ushort number, CancellationToken cancellationToken)
  {
    ReceiptItem? item = await _mediator.Send(new RemoveReceiptItemCommand(receiptId, number), cancellationToken);
    return item == null ? NotFound() : Ok(item);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ReceiptItem>>> SearchAsync(Guid receiptId, [FromQuery] SearchReceiptItemsModel model, CancellationToken cancellationToken)
  {
    SearchResults<ReceiptItem> results = await _mediator.Send(new SearchReceiptItemsQuery(model.ToPayload(receiptId)), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{number}")]
  public async Task<ActionResult<ReceiptItem>> UpdateAsync(Guid receiptId, ushort number, [FromBody] UpdateReceiptItemPayload payload, CancellationToken cancellationToken)
  {
    ReceiptItem? item = await _mediator.Send(new UpdateReceiptItemCommand(receiptId, number, payload), cancellationToken);
    return item == null ? NotFound() : Ok(item);
  }
}
