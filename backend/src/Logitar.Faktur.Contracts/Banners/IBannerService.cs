using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Banners;

public interface IBannerService
{
  Task<AcceptedCommand> CreateAsync(CreateBannerPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<Banner?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> ReplaceAsync(string id, ReplaceBannerPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> UpdateAsync(string id, UpdateBannerPayload payload, CancellationToken cancellationToken = default);
}
