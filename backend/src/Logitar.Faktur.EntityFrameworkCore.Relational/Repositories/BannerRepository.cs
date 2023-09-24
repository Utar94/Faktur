using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Domain.Banners;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Repositories;

internal class BannerRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IBannerRepository
{
  public BannerRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<BannerAggregate?> LoadAsync(BannerId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<BannerAggregate?> LoadAsync(BannerId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<BannerAggregate>(id.AggregateId, version, cancellationToken);

  public async Task SaveAsync(BannerAggregate banner, CancellationToken cancellationToken)
    => await base.SaveAsync(banner, cancellationToken);
  public async Task SaveAsync(IEnumerable<BannerAggregate> banners, CancellationToken cancellationToken)
    => await base.SaveAsync(banners, cancellationToken);
}
