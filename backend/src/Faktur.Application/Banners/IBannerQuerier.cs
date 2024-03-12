using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Banners;

public interface IBannerQuerier
{
  Task<Banner> ReadAsync(BannerAggregate banner, CancellationToken cancellationToken = default);
  Task<Banner?> ReadAsync(BannerId id, CancellationToken cancellationToken = default);
  Task<Banner?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken = default);
}
