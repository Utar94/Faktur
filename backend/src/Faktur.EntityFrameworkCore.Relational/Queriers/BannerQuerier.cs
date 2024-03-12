using Faktur.Application.Banners;
using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class BannerQuerier : IBannerQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<BannerEntity> _banners;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public BannerQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _banners = context.Banners;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Banner> ReadAsync(BannerAggregate banner, CancellationToken cancellationToken)
  {
    return await ReadAsync(banner.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The banner 'AggregateId={banner.Id.Value}' could not be found.");
  }
  public async Task<Banner?> ReadAsync(BannerId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Banner?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    BannerEntity? banner = await _banners.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return banner == null ? null : await MapAsync(banner, cancellationToken);
  }

  public async Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Banners.Table).SelectAll(FakturDb.Banners.Table)
      .ApplyIdFilter(FakturDb.Banners.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Banners.DisplayName);

    IQueryable<BannerEntity> query = _banners.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<BannerEntity>? ordered = null;
    foreach (BannerSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case BannerSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case BannerSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    BannerEntity[] banners = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Banner> items = await MapAsync(banners, cancellationToken);

    return new SearchResults<Banner>(items, total);
  }

  private async Task<Banner> MapAsync(BannerEntity banner, CancellationToken cancellationToken)
  {
    return (await MapAsync([banner], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Banner>> MapAsync(IEnumerable<BannerEntity> banners, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = banners.SelectMany(banner => banner.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return banners.Select(mapper.ToBanner);
  }
}
