using Faktur.Application.Products.Commands;
using Faktur.Application.Products.Queries;
using Faktur.Contracts.Products;
using Faktur.Extensions;
using Faktur.Models.Products;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("stores/{storeId}/products")]
public class ProductController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProductController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPut("{articleId}")]
  public async Task<ActionResult<Product>> CreateOrReplaceAsync(Guid storeId, Guid articleId, [FromBody] CreateOrReplaceProductPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceProductResult result = await _mediator.Send(new CreateOrReplaceProductCommand(storeId, articleId, payload, version), cancellationToken);
    if (result.IsCreated)
    {
      Product product = result.Product;
      Uri location = HttpContext.BuildLocation("stores/{storeId}/products/{articleId}", new Dictionary<string, string>
      {
        ["storeId"] = product.Store.Id.ToString(),
        ["articleId"] = product.Article.Id.ToString()
      });
      return Created(location, product);
    }
    else
    {
      return Ok(result.Product);
    }
  }

  [HttpDelete("{articleId}")]
  public async Task<ActionResult<Product>> DeleteAsync(Guid storeId, Guid articleId, CancellationToken cancellationToken)
  {
    Product? product = await _mediator.Send(new DeleteProductCommand(storeId, articleId), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet("{articleId}")]
  public async Task<ActionResult<Product>> ReadAsync(Guid storeId, Guid articleId, CancellationToken cancellationToken)
  {
    Product? product = await _mediator.Send(new ReadProductQuery(Id: null, storeId, articleId, Sku: null), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet("sku:{sku}")]
  public async Task<ActionResult<Product>> ReadAsync(Guid storeId, string sku, CancellationToken cancellationToken)
  {
    Product? product = await _mediator.Send(new ReadProductQuery(Id: null, storeId, ArticleId: null, sku), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet]
  public async Task<ActionResult<Product>> SearchAsync(Guid storeId, [FromQuery] SearchProductsModel model, CancellationToken cancellationToken)
  {
    SearchResults<Product> results = await _mediator.Send(new SearchProductsQuery(model.ToPayload(storeId)), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{articleId}")]
  public async Task<ActionResult<Product>> UpdateAsync(Guid storeId, Guid articleId, [FromBody] UpdateProductPayload payload, CancellationToken cancellationToken)
  {
    Product? product = await _mediator.Send(new UpdateProductCommand(storeId, articleId, payload), cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }
}
