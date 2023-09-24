namespace Logitar.Faktur.Domain.Banners;

public interface IBannerRepository
{
  Task<BannerAggregate?> LoadAsync(BannerId id, CancellationToken cancellationToken = default);
  Task<BannerAggregate?> LoadAsync(BannerId id, long? version, CancellationToken cancellationToken = default);
  Task SaveAsync(BannerAggregate banner, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<BannerAggregate> banners, CancellationToken cancellationToken = default);
}
