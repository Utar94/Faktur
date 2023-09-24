using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Application.Banners;

public interface IBannerQuerier
{
  Task<Banner?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken = default);
}
