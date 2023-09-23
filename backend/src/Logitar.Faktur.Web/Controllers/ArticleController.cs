using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Controllers;

[ApiController]
[Route("articles")]
public class ArticleController : ControllerBase
{
  private readonly IArticleService articleService;

  public ArticleController(IArticleService articleService)
  {
    this.articleService = articleService;
  }

  [HttpPost]
  public async Task<ActionResult<AcceptedCommand>> CreateAsync([FromBody] CreateArticlePayload payload, CancellationToken cancellationToken)
  {
    AcceptedCommand result = await articleService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.GetBaseUrl()}/articles/{result.AggregateId}");

    return Accepted(uri, result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<AcceptedCommand>> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return Accepted(await articleService.DeleteAsync(id, cancellationToken));
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Article>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Article? article = await articleService.ReadAsync(id, cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AcceptedCommand>> ReplaceAsync(string id, [FromBody] ReplaceArticlePayload payload, long? version, CancellationToken cancellationToken)
  {
    return Accepted(await articleService.ReplaceAsync(id, payload, version, cancellationToken));
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Article>>> SearchAsync(CancellationToken cancellationToken)
  {
    return Ok(await articleService.SearchAsync(payload: new(), cancellationToken)); // TODO(fpion): implement
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AcceptedCommand>> UpdateAsync(string id, [FromBody] UpdateArticlePayload payload, CancellationToken cancellationToken)
  {
    return Accepted(await articleService.UpdateAsync(id, payload, cancellationToken));
  }
}
