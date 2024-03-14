using Faktur.Application.Products;
using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ProductQuerier : IProductQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ProductEntity> _products;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ProductQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _products = context.Products;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Product> ReadAsync(ProductAggregate product, CancellationToken cancellationToken)
  {
    return await ReadAsync(product.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The product 'AggregateId={product.Id.Value}' could not be found.");
  }
  public async Task<Product?> ReadAsync(ProductId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Product?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ProductEntity? product = await _products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return product == null ? null : await MapAsync(product, cancellationToken);
  }

  public async Task<Product?> ReadAsync(Guid storeGuid, Guid articleGuid, CancellationToken cancellationToken)
  {
    StoreId storeId = new(storeGuid);
    ArticleId articleId = new(articleGuid);

    ProductEntity? product = await _products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == storeId.Value && x.Article!.AggregateId == articleId.Value, cancellationToken);

    return product == null ? null : await MapAsync(product, cancellationToken);
  }
  public async Task<Product?> ReadAsync(Guid storeGuid, string sku, CancellationToken cancellationToken)
  {
    StoreId storeId = new(storeGuid);
    string skuNormalized = sku.Trim().ToUpper();

    ProductEntity? product = await _products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store).ThenInclude(x => x!.Banner)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == storeId.Value && x.SkuNormalized == skuNormalized, cancellationToken);

    return product == null ? null : await MapAsync(product, cancellationToken);
  }

  public async Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken)
  {
    StoreId storeId = new(payload.StoreId);

    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Products.Table).SelectAll(FakturDb.Products.Table)
      .Join(FakturDb.Stores.StoreId, FakturDb.Products.StoreId)
      .LeftJoin(FakturDb.Departments.DepartmentId, FakturDb.Products.DepartmentId)
      .ApplyIdFilter(FakturDb.Products.AggregateId, payload.Ids)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(storeId.Value));
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Products.Sku, FakturDb.Products.DisplayName);

    if (!string.IsNullOrWhiteSpace(payload.DepartmentNumber))
    {
      string numberNormalized = payload.DepartmentNumber.Trim().ToUpper();
      builder.Where(FakturDb.Departments.NumberNormalized, Operators.IsEqualTo(numberNormalized));
    }
    if (payload.UnitType.HasValue)
    {
      builder.Where(FakturDb.Products.UnitType, Operators.IsEqualTo(payload.UnitType.Value.ToString()));
    }

    IQueryable<ProductEntity> query = _products.FromQuery(builder).AsNoTracking()
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
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ProductSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ProductEntity[] products = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Product> items = await MapAsync(products, cancellationToken);

    return new SearchResults<Product>(items, total);
  }

  private async Task<Product> MapAsync(ProductEntity product, CancellationToken cancellationToken)
  {
    return (await MapAsync([product], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Product>> MapAsync(IEnumerable<ProductEntity> products, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = products.SelectMany(product => product.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return products.Select(mapper.ToProduct);
  }
}
