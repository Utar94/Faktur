using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Web.Extensions;
using Logitar.Faktur.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Controllers;

[ApiController]
[Route("stores/{storeId}/products")]
public class ProductController : ControllerBase
{
  private readonly IProductService productService;

  public ProductController(IProductService productService)
  {
    this.productService = productService;
  }

  [HttpGet("{articleId}")]
  public async Task<ActionResult<Product>> ReadAsync(string storeId, string articleId, CancellationToken cancellationToken)
  {
    Product? product = await productService.ReadAsync(storeId, articleId, cancellationToken: cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpGet("sku:{sku}")]
  public async Task<ActionResult<Product>> ReadBySkuAsync(string storeId, string sku, CancellationToken cancellationToken)
  {
    Product? product = await productService.ReadAsync(storeId, sku: sku, cancellationToken: cancellationToken);
    return product == null ? NotFound() : Ok(product);
  }

  [HttpDelete("{articleId}")]
  public async Task<ActionResult<AcceptedCommand>> RemoveAsync(string storeId, string articleId, CancellationToken cancellationToken)
  {
    return Accepted(await productService.RemoveAsync(storeId, articleId, cancellationToken));
  }

  [HttpPut("{articleId}")]
  public async Task<ActionResult<AcceptedCommand>> SaveAsync(string storeId, string articleId,
    [FromBody] SaveProductPayload payload, CancellationToken cancellationToken)
  {
    AcceptedCommand result = await productService.SaveAsync(storeId, articleId, payload, cancellationToken);
    Uri uri = new($"{Request.GetBaseUrl()}/stores/{storeId}/products/{articleId}");

    return Accepted(uri, result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Product>>> SearchAsync(string storeId, [FromQuery] SearchProductsQuery query, CancellationToken cancellationToken)
  {
    return Ok(await productService.SearchAsync(query.ToPayload(storeId), cancellationToken));
  }

  [HttpPatch("{articleId}")]
  public async Task<ActionResult<AcceptedCommand>> UpdateAsync(string storeId, string articleId,
    [FromBody] UpdateProductPayload payload, CancellationToken cancellationToken)
  {
    return Accepted(await productService.UpdateAsync(storeId, articleId, payload, cancellationToken));
  }
}
