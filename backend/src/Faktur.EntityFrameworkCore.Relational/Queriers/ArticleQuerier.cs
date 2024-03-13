using Faktur.Application.Articles;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ArticleQuerier : IArticleQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ArticleEntity> _articles;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ArticleQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _articles = context.Articles;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Article> ReadAsync(ArticleAggregate article, CancellationToken cancellationToken)
  {
    return await ReadAsync(article.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The article 'AggregateId={article.Id.Value}' could not be found.");
  }
  public async Task<Article?> ReadAsync(ArticleId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Article?> ReadAsync(string gtin, CancellationToken cancellationToken)
  {
    ArticleEntity? article = null;

    if (long.TryParse(gtin.Trim(), out long gtinNormalized))
    {
      article = await _articles.AsNoTracking().SingleOrDefaultAsync(x => x.GtinNormalized == gtinNormalized, cancellationToken);
    }

    return article == null ? null : await MapAsync(article, cancellationToken);
  }
  public async Task<Article?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ArticleEntity? article = await _articles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return article == null ? null : await MapAsync(article, cancellationToken);
  }

  public async Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Articles.Table).SelectAll(FakturDb.Articles.Table)
      .ApplyIdFilter(FakturDb.Articles.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Articles.Gtin, FakturDb.Articles.DisplayName);

    IQueryable<ArticleEntity> query = _articles.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ArticleEntity>? ordered = null;
    foreach (ArticleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ArticleSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ArticleSort.Gtin:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.GtinNormalized) : query.OrderBy(x => x.GtinNormalized))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.GtinNormalized) : ordered.ThenBy(x => x.GtinNormalized));
          break;
        case ArticleSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ArticleEntity[] articles = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Article> items = await MapAsync(articles, cancellationToken);

    return new SearchResults<Article>(items, total);
  }

  private async Task<Article> MapAsync(ArticleEntity article, CancellationToken cancellationToken)
  {
    return (await MapAsync([article], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Article>> MapAsync(IEnumerable<ArticleEntity> articles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = articles.SelectMany(article => article.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return articles.Select(mapper.ToArticle);
  }
}
