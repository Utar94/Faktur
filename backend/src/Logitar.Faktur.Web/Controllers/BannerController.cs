using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Web.Extensions;
using Logitar.Faktur.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Controllers;

[ApiController]
[Route("banners")]
public class BannerController : ControllerBase
{
  private readonly IBannerService bannerService;

  public BannerController(IBannerService bannerService)
  {
    this.bannerService = bannerService;
  }

  [HttpPost]
  public async Task<ActionResult<AcceptedCommand>> CreateAsync([FromBody] CreateBannerPayload payload, CancellationToken cancellationToken)
  {
    AcceptedCommand result = await bannerService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.GetBaseUrl()}/banners/{result.AggregateId}");

    return Accepted(uri, result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<AcceptedCommand>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return Accepted(await bannerService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Banner>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Banner? banner = await bannerService.ReadAsync(id, cancellationToken);
    return banner == null ? NotFound() : Ok(banner);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AcceptedCommand>> ReplaceAsync(string id, [FromBody] ReplaceBannerPayload payload, long? version, CancellationToken cancellationToken)
  {
    return Accepted(await bannerService.ReplaceAsync(id, payload, version, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Banner>>> SearchAsync([FromQuery] SearchBannersQuery query, CancellationToken cancellationToken)
  {
    return Ok(await bannerService.SearchAsync(query.ToPayload(), cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AcceptedCommand>> UpdateAsync(string id, [FromBody] UpdateBannerPayload payload, CancellationToken cancellationToken)
  {
    return Accepted(await bannerService.UpdateAsync(id, payload, cancellationToken));
  }
}
