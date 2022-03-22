using Faktur.Core.Models;
using Faktur.Core.Receipts;
using Faktur.Core.Receipts.Commands;
using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Payloads;
using Faktur.Core.Receipts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Web.Controllers
{
  [ApiController]
  [Authorize]
  [Route("receipts")]
  public class ReceiptController : ControllerBase
  {
    private readonly IMediator mediator;

    public ReceiptController(IMediator mediator)
    {
      this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<ReceiptModel>>> GetAsync(
      bool? deleted,
      string? search,
      int? storeId,
      ReceiptSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      return Ok(await mediator.Send(new GetReceipts
      {
        Deleted = deleted,
        Search = search,
        StoreId = storeId,
        Sort = sort,
        Desc = desc,
        Index = index,
        Count = count
      }, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReceiptModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new GetReceipt(id), cancellationToken));
    }

    [HttpPost("import")]
    public async Task<ActionResult<ReceiptModel>> ImportAsync(
      [FromBody] ImportReceiptPayload payload,
      CancellationToken cancellationToken
    )
    {
      return Ok(await mediator.Send(new ImportReceipt(payload), cancellationToken));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(
      int id,
      [FromBody] UpdateReceiptPayload payload,
      CancellationToken cancellationToken
    )
    {
      return Ok(await mediator.Send(new UpdateReceipt(id, payload), cancellationToken));
    }

    [HttpPatch("{id}/delete")]
    public async Task<ActionResult<ReceiptModel>> SetDeletedAsync(int id, CancellationToken cancellationToken)
    {
      return Ok(await mediator.Send(new DeleteReceipt(id), cancellationToken));
    }

    [HttpPut("{id}/process")]
    public async Task<ActionResult> ProcessAsync(
      int id,
      [FromBody] ProcessReceiptPayload payload,
      CancellationToken cancellationToken
    )
    {
      await mediator.Send(new ProcessReceipt(id, payload), cancellationToken);

      return NoContent();
    }

    [HttpPut("items/{id}")]
    public async Task<ActionResult<ReceiptModel>> UpdateItemAsync(
      int id,
      [FromBody] UpdateItemPayload payload,
      CancellationToken cancellationToken
    )
    {
      return Ok(await mediator.Send(new UpdateItem(id, payload), cancellationToken));
    }
  }
}
