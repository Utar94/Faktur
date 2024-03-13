using Faktur.Application.Articles.Commands;
using Faktur.Application.Articles.Queries;
using Faktur.Contracts.Articles;
using Faktur.Extensions;
using Faktur.Models.Articles;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("articles")]
public class ArticleController : ControllerBase
{
  private readonly IMediator _mediator;

  public ArticleController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<Article>> CreateAsync([FromBody] CreateArticlePayload payload, CancellationToken cancellationToken)
  {
    Article article = await _mediator.Send(new CreateArticleCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("articles/{id}", new Dictionary<string, string> { ["id"] = article.Id.ToString() });
    return Created(location, article);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Article>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Article? article = await _mediator.Send(new DeleteArticleCommand(id), cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Article>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Article? article = await _mediator.Send(new ReadArticleQuery(id, Gtin: null), cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }

  [HttpGet("gtin:{gtin}")]
  public async Task<ActionResult<Article>> ReadAsync(string gtin, CancellationToken cancellationToken)
  {
    Article? article = await _mediator.Send(new ReadArticleQuery(Id: null, gtin), cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Article>> ReplaceAsync(Guid id, [FromBody] ReplaceArticlePayload payload, long? version, CancellationToken cancellationToken)
  {
    Article? article = await _mediator.Send(new ReplaceArticleCommand(id, payload, version), cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Article>>> SearchAsync([FromQuery] SearchArticlesModel model, CancellationToken cancellationToken)
  {
    SearchResults<Article> results = await _mediator.Send(new SearchArticlesQuery(model.ToPayload()), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Article>> UpdateAsync(Guid id, [FromBody] UpdateArticlePayload payload, CancellationToken cancellationToken)
  {
    Article? article = await _mediator.Send(new UpdateArticleCommand(id, payload), cancellationToken);
    return article == null ? NotFound() : Ok(article);
  }
}
