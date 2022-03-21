using AutoMapper;
using Faktur.Core;
using Faktur.Core.Articles;
using Faktur.Core.Models;
using Faktur.Core.Products;
using Faktur.Core.Products.Models;
using Faktur.Core.Products.Payloads;
using Faktur.Core.Stores;
using Faktur.Infrastructure;
using Logitar;
using Logitar.Identity.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Web.Controllers
{
  [ApiController]
  [Authorize]
  [Route("products")]
  public class ProductController : ControllerBase
  {
    private readonly FakturDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public ProductController(FakturDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.userContext = userContext;
    }

    [HttpPost]
    public async Task<ActionResult<ProductModel>> CreateAsync(
      [FromBody] CreateProductPayload payload,
      CancellationToken cancellationToken
    )
    {
      Article article = await dbContext.Articles
        .SingleOrDefaultAsync(x => x.Id == payload.ArticleId, cancellationToken)
        ?? throw new EntityNotFoundException<Article>(payload.ArticleId, nameof(payload.ArticleId));

      Store store = await dbContext.Stores
        .Include(x => x.Products)
        .SingleOrDefaultAsync(x => x.Id == payload.StoreId, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(payload.StoreId, nameof(payload.StoreId));

      if (store.Products.Any(x => x.ArticleId == article.Id))
      {
        throw new ProductAlreadyExistingException(store.Id, article.Id, nameof(payload.ArticleId));
      }

      var product = new Product(article, store, userContext.Id);
      dbContext.Products.Add(product);

      ProductModel model = await SaveAsync(product, payload, store.Products, cancellationToken);
      var uri = new Uri($"/products/{model.Id}");

      return Created(uri, model);
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<ProductModel>>> GetAsync(
      int? articleId,
      bool? deleted,
      int? departmentId,
      string? search,
      int? storeId,
      ProductSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      IQueryable<Product> query = dbContext.Products
        .AsNoTracking()
        .Include(x => x.Article);

      if (articleId.HasValue)
      {
        query = query.Where(x => x.ArticleId == articleId.Value);
      }
      if (deleted.HasValue)
      {
        query = query.Where(x => x.Deleted == deleted.Value);
      }
      if (departmentId.HasValue)
      {
        query = query.Where(x => x.DepartmentId == departmentId.Value);
      }
      if (search != null)
      {
        query = query.Where(x => (x.Label != null && x.Label.Contains(search))
          || (x.Sku != null && x.Sku.Contains(search))
          || x.Article!.Name.Contains(search));
      }
      if (storeId.HasValue)
      {
        query = query.Where(x => x.StoreId == storeId.Value);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        switch (sort.Value)
        {
          case ProductSort.Label:
            query = desc ? query.OrderByDescending(x => x.Label ?? x.Article!.Name) : query.OrderBy(x => x.Label ?? x.Article!.Name);
            break;
          case ProductSort.Sku:
            query = desc ? query.OrderByDescending(x => x.Sku) : query.OrderBy(x => x.Sku);
            break;
          case ProductSort.UpdatedAt:
            query = desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt);
            break;
          default:
            return BadRequest(new { code = "invalid_sort" });
        }
      }

      query = query.ApplyPaging(index, count);

      Product[] products = await query.ToArrayAsync(cancellationToken);

      return Ok(new ListModel<ProductModel>(mapper.Map<IEnumerable<ProductModel>>(products), total));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProductModel>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
      Product product = await dbContext.Products
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Product>(id);

      dbContext.Products.Remove(product);
      await dbContext.SaveChangesAsync(cancellationToken);

      return Ok(mapper.Map<ProductModel>(product));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      Product product = await dbContext.Products
        .AsNoTracking()
        .Include(x => x.Article)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Product>(id);

      return Ok(mapper.Map<ProductModel>(product));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductModel>> UpdateAsync(
      int id,
      [FromBody] UpdateProductPayload payload,
      CancellationToken cancellationToken
    )
    {
      Product product = await dbContext.Products
        .Include(x => x.Article)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Product>(id);

      Product[] storeProducts = await dbContext.Products
        .AsNoTracking()
        .Where(x => x.StoreId == product.StoreId)
        .ToArrayAsync(cancellationToken);

      product.Update(userContext.Id);

      return Ok(await SaveAsync(product, payload, storeProducts, cancellationToken));
    }

    private async Task<ProductModel> SaveAsync(Product product, SaveProductPayload payload, IEnumerable<Product> storeProducts, CancellationToken cancellationToken)
    {
      if (payload.Sku != null)
      {
        Product? conflict = storeProducts.SingleOrDefault(x => x.Sku == payload.Sku);
        if (conflict?.Equals(product) == false)
        {
          throw new SkuAlreadyTakenException(product.StoreId, payload.Sku, nameof(payload.Sku));
        }
      }

      Department? department = payload.DepartmentId.HasValue
        ? await dbContext.Departments
          .SingleOrDefaultAsync(x => x.Id == payload.DepartmentId.Value, cancellationToken)
          ?? throw new EntityNotFoundException<Department>(payload.DepartmentId.Value, nameof(payload.DepartmentId))
        : null;

      product.Department = department;
      product.DepartmentId = department?.Id;
      product.Description = payload.Description?.CleanTrim();
      product.Flags = payload.Flags?.CleanTrim();
      product.Label = payload.Label?.CleanTrim();
      product.Sku = payload.Sku?.CleanTrim();
      product.UnitPrice = payload.UnitPrice;
      product.UnitType = payload.UnitType?.CleanTrim();

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ProductModel>(product);
    }
  }
}
