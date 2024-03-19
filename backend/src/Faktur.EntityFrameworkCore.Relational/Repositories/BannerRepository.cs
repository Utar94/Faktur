using Faktur.Domain.Banners;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class BannerRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IBannerRepository
{
  public BannerRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<BannerAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<BannerAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<BannerAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<BannerAggregate>(new AggregateId(id), version, cancellationToken);
  }

  public async Task<IEnumerable<BannerAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<BannerAggregate>(cancellationToken);
  }

  public async Task SaveAsync(BannerAggregate banner, CancellationToken cancellationToken)
  {
    await base.SaveAsync(banner, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<BannerAggregate> banners, CancellationToken cancellationToken)
  {
    await base.SaveAsync(banners, cancellationToken);
  }
}
