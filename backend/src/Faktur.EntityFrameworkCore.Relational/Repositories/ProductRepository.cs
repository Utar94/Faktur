using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ProductRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IProductRepository
{
  private static readonly string AggregateType = typeof(ProductAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ProductRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ProductAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProductAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<ProductAggregate?> LoadAsync(ProductId id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProductAggregate>(id.AggregateId, version, cancellationToken);
  }

  public async Task<IEnumerable<ProductAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProductAggregate>(cancellationToken);
  }

  public async Task<IEnumerable<ProductAggregate>> LoadAsync(ArticleAggregate article, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Products.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Articles.ArticleId, FakturDb.Products.ArticleId)
      .Where(FakturDb.Articles.AggregateId, Operators.IsEqualTo(article.Id.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProductAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<IEnumerable<ProductAggregate>> LoadAsync(StoreAggregate store, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Products.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Stores.StoreId, FakturDb.Products.StoreId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(store.Id.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProductAggregate>(events.Select(EventSerializer.Deserialize));
  }
  public async Task<IEnumerable<ProductAggregate>> LoadAsync(StoreAggregate store, NumberUnit departmentNumber, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Products.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Departments.DepartmentId, FakturDb.Products.DepartmentId)
      .Join(FakturDb.Stores.StoreId, FakturDb.Departments.StoreId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(store.Id.Value))
      .Where(FakturDb.Departments.NumberNormalized, Operators.IsEqualTo(departmentNumber.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProductAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task<ProductAggregate?> LoadAsync(Guid storeGuid, Guid articleGuid, CancellationToken cancellationToken)
  {
    StoreId storeId = new(storeGuid);
    ArticleId articleId = new(articleGuid);

    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Products.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Stores.StoreId, FakturDb.Products.StoreId)
      .Join(FakturDb.Articles.ArticleId, FakturDb.Products.ArticleId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(storeId.Value))
      .Where(FakturDb.Articles.AggregateId, Operators.IsEqualTo(articleId.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProductAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<ProductAggregate?> LoadAsync(StoreId storeId, SkuUnit sku, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Products.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Stores.StoreId, FakturDb.Products.StoreId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(storeId.Value))
      .Where(FakturDb.Products.SkuNormalized, Operators.IsEqualTo(sku.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProductAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ProductAggregate product, CancellationToken cancellationToken)
  {
    await base.SaveAsync(product, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ProductAggregate> products, CancellationToken cancellationToken)
  {
    await base.SaveAsync(products, cancellationToken);
  }
}
