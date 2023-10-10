using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Banners;

namespace Logitar.Faktur.Application.Banners;

public interface IBannerQuerier
{
  Task<Banner?> ReadAsync(BannerId id, CancellationToken cancellationToken = default);
  Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken = default);
}
