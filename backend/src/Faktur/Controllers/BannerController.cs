using Faktur.Application.Banners.Commands;
using Faktur.Application.Banners.Queries;
using Faktur.Contracts.Banners;
using Faktur.Extensions;
using Faktur.Models.Banners;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("banners")]
public class BannerController : ControllerBase
{
  private readonly IMediator _mediator;

  public BannerController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<Banner>> CreateAsync([FromBody] CreateBannerPayload payload, CancellationToken cancellationToken)
  {
    Banner banner = await _mediator.Send(new CreateBannerCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("banners/{id}", new Dictionary<string, string> { ["id"] = banner.Id.ToString() });
    return Created(location, banner);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Banner>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Banner? banner = await _mediator.Send(new DeleteBannerCommand(id), cancellationToken);
    return banner == null ? NotFound() : Ok(banner);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Banner>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Banner? banner = await _mediator.Send(new ReadBannerQuery(id), cancellationToken);
    return banner == null ? NotFound() : Ok(banner);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Banner>> ReplaceAsync(Guid id, [FromBody] ReplaceBannerPayload payload, long? version, CancellationToken cancellationToken)
  {
    Banner? banner = await _mediator.Send(new ReplaceBannerCommand(id, payload, version), cancellationToken);
    return banner == null ? NotFound() : Ok(banner);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Banner>>> SearchAsync([FromQuery] SearchBannersModel model, CancellationToken cancellationToken)
  {
    SearchResults<Banner> results = await _mediator.Send(new SearchBannersQuery(model.ToPayload()), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Banner>> UpdateAsync(Guid id, [FromBody] UpdateBannerPayload payload, CancellationToken cancellationToken)
  {
    Banner? banner = await _mediator.Send(new UpdateBannerCommand(id, payload), cancellationToken);
    return banner == null ? NotFound() : Ok(banner);
  }
}
