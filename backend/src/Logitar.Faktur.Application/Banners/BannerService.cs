using Logitar.Faktur.Application.Banners.Commands;
using Logitar.Faktur.Application.Banners.Queries;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Banners;

internal class BannerService : IBannerService
{
  private readonly IMediator mediator;

  public BannerService(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public async Task<AcceptedCommand> CreateAsync(CreateBannerPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new CreateBannerCommand(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new DeleteBannerCommand(id), cancellationToken);
  }

  public async Task<Banner?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReadBannerQuery(id), cancellationToken);
  }

  public async Task<AcceptedCommand> ReplaceAsync(string id, ReplaceBannerPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReplaceBannerCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Banner>> SearchAsync(SearchBannersPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SearchBannersQuery(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> UpdateAsync(string id, UpdateBannerPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new UpdateBannerCommand(id, payload), cancellationToken);
  }
}
