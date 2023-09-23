using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Articles;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ArticleQuerier : IArticleQuerier
{
  private readonly IActorService actorService;
  private readonly DbSet<ArticleEntity> articles;
  private readonly ISqlHelper sqlHelper;

  public ArticleQuerier(IActorService actorService, FakturContext context, ISqlHelper sqlHelper)
  {
    this.actorService = actorService;
    articles = context.Articles;
    this.sqlHelper = sqlHelper;
  }

  public async Task<Article?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    id = id.Trim();

    ArticleEntity? article = await articles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return article == null ? null : await MapAsync(article, cancellationToken);
  }

  public async Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = sqlHelper.QueryFrom(Db.Articles.Table).SelectAll(Db.Articles.Table);
    sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Articles.AggregateId);
    sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Articles.Gtin, Db.Articles.DisplayName);

    IQueryable<ArticleEntity> query = this.articles.FromQuery(builder);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ArticleEntity>? ordered = null;
    foreach (ArticleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ArticleSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.DisplayName) : ordered.OrderBy(x => x.DisplayName));
          break;
        case ArticleSort.Gtin:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.GtinNormalized) : query.OrderBy(x => x.GtinNormalized))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.GtinNormalized) : ordered.OrderBy(x => x.GtinNormalized));
          break;
        case ArticleSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.UpdatedOn) : ordered.OrderBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ArticleEntity[] articles = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Article> results = await MapAsync(articles, cancellationToken);

    return new SearchResults<Article>(results, total);
  }

  private async Task<Article> MapAsync(ArticleEntity article, CancellationToken cancellationToken)
    => (await MapAsync(new[] { article }, cancellationToken)).Single();
  private async Task<IEnumerable<Article>> MapAsync(IEnumerable<ArticleEntity> articles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> ids = articles.SelectMany(article => article.GetActorIds());
    IEnumerable<Actor> actors = await actorService.FindAsync(ids, cancellationToken);
    Mapper mapper = new(actors);

    return articles.Select(mapper.ToArticle);
  }
}
