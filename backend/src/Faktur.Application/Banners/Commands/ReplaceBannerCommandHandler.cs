using Faktur.Application.Banners.Validators;
using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class ReplaceBannerCommandHandler : IRequestHandler<ReplaceBannerCommand, Banner?>
{
  private readonly IBannerQuerier _bannerQuerier;
  private readonly IBannerRepository _bannerRepository;

  public ReplaceBannerCommandHandler(IBannerQuerier bannerQuerier, IBannerRepository bannerRepository)
  {
    _bannerQuerier = bannerQuerier;
    _bannerRepository = bannerRepository;
  }

  public async Task<Banner?> Handle(ReplaceBannerCommand command, CancellationToken cancellationToken)
  {
    ReplaceBannerPayload payload = command.Payload;
    new ReplaceBannerValidator().ValidateAndThrow(payload);

    BannerAggregate? banner = await _bannerRepository.LoadAsync(command.Id, cancellationToken);
    if (banner == null)
    {
      return null;
    }
    BannerAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _bannerRepository.LoadAsync(command.Id, command.Version.Value, cancellationToken);
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      banner.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      banner.Description = description;
    }

    banner.Update(command.ActorId);

    await _bannerRepository.SaveAsync(banner, cancellationToken);

    return await _bannerQuerier.ReadAsync(banner, cancellationToken);
  }
}
