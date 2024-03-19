namespace Faktur.Domain.Banners;

public interface IBannerRepository
{
  Task<BannerAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<BannerAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken = default);
  Task<IEnumerable<BannerAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task SaveAsync(BannerAggregate banner, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<BannerAggregate> banners, CancellationToken cancellationToken = default);
}
