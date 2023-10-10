using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Products;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ProductQuerier : IProductQuerier
{
  private readonly IActorService actorService;
  private readonly DbSet<ProductEntity> products;
  private readonly ISqlHelper sqlHelper;

  public ProductQuerier(IActorService actorService, FakturContext context, ISqlHelper sqlHelper)
  {
    this.actorService = actorService;
    products = context.Products;
    this.sqlHelper = sqlHelper;
  }

  public async Task<Product?> ReadAsync(StoreId storeId, ArticleId articleId, CancellationToken cancellationToken)
  {
    ProductEntity? product = await products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == storeId.Value && x.Article!.AggregateId == articleId.Value, cancellationToken);

    return product == null ? null : await MapAsync(product, cancellationToken);
  }

  public async Task<Product?> ReadAsync(StoreId storeId, SkuUnit sku, CancellationToken cancellationToken)
  {
    string skuNormalized = sku.Value.ToUpper();

    ProductEntity? product = await products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == storeId.Value && x.SkuNormalized == skuNormalized, cancellationToken);

    return product == null ? null : await MapAsync(product, cancellationToken);
  }

  public async Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken)
  {
    StoreId storeId = StoreId.Parse(payload.StoreId, nameof(payload.StoreId));
    DepartmentNumberUnit? departmentNumber = string.IsNullOrWhiteSpace(payload.DepartmentNumber)
      ? null : DepartmentNumberUnit.Parse(payload.DepartmentNumber, nameof(payload.DepartmentNumber));

    IQueryBuilder builder = sqlHelper.QueryFrom(Db.Products.Table)
      .Join(Db.Articles.ArticleId, Db.Products.ArticleId)
      .Join(Db.Stores.StoreId, Db.Products.StoreId)
      .LeftJoin(Db.Departments.DepartmentId, Db.Products.DepartmentId)
      .Where(Db.Stores.AggregateId, Operators.IsEqualTo(storeId.Value))
      .Where(Db.Departments.Number, departmentNumber == null ? Operators.IsNull() : Operators.IsEqualTo(departmentNumber.Value))
      .SelectAll(Db.Products.Table);
    sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Articles.AggregateId);
    sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Products.Sku, Db.Products.DisplayName);

    IQueryable<ProductEntity> query = this.products.FromQuery(builder).AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ProductEntity>? ordered = null;
    foreach (ProductSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ProductSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.DisplayName) : ordered.OrderBy(x => x.DisplayName));
          break;
        case ProductSort.Sku:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.SkuNormalized) : query.OrderBy(x => x.SkuNormalized))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.SkuNormalized) : ordered.OrderBy(x => x.SkuNormalized));
          break;
        case ProductSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.UpdatedOn) : ordered.OrderBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ProductEntity[] products = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Product> results = await MapAsync(products, cancellationToken);

    return new SearchResults<Product>(results, total);
  }

  private async Task<Product> MapAsync(ProductEntity product, CancellationToken cancellationToken)
    => (await MapAsync(new[] { product }, cancellationToken)).Single();
  private async Task<IEnumerable<Product>> MapAsync(IEnumerable<ProductEntity> products, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> ids = products.SelectMany(product => product.GetActorIds());
    IEnumerable<Actor> actors = await actorService.FindAsync(ids, cancellationToken);
    Mapper mapper = new(actors);

    return products.Select(mapper.ToProduct);
  }
}
