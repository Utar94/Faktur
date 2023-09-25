using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Banners;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Queriers;

internal class BannerQuerier : IBannerQuerier
{
  private readonly IActorService actorService;
  private readonly DbSet<BannerEntity> banners;
  private readonly ISqlHelper sqlHelper;

  public BannerQuerier(IActorService actorService, FakturContext context, ISqlHelper sqlHelper)
  {
    this.actorService = actorService;
    banners = context.Banners;
    this.sqlHelper = sqlHelper;
  }

  public async Task<Banner?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    id = id.Trim();

    BannerEntity? banner = await banners.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return banner == null ? null : await MapAsync(banner, cancellationToken);
  }

  public async Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = sqlHelper.QueryFrom(Db.Banners.Table).SelectAll(Db.Banners.Table);
    sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Banners.AggregateId);
    sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Banners.DisplayName);

    IQueryable<BannerEntity> query = this.banners.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<BannerEntity>? ordered = null;
    foreach (BannerSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case BannerSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.DisplayName) : ordered.OrderBy(x => x.DisplayName));
          break;
        case BannerSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.UpdatedOn) : ordered.OrderBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    BannerEntity[] banners = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Banner> results = await MapAsync(banners, cancellationToken);

    return new SearchResults<Banner>(results, total);
  }

  private async Task<Banner> MapAsync(BannerEntity banner, CancellationToken cancellationToken)
    => (await MapAsync(new[] { banner }, cancellationToken)).Single();
  private async Task<IEnumerable<Banner>> MapAsync(IEnumerable<BannerEntity> banners, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> ids = banners.SelectMany(banner => banner.GetActorIds());
    IEnumerable<Actor> actors = await actorService.FindAsync(ids, cancellationToken);
    Mapper mapper = new(actors);

    return banners.Select(mapper.ToBanner);
  }
}
