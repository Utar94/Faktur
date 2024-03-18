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
}
