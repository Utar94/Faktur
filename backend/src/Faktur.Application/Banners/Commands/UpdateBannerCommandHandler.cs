using Faktur.Application.Banners.Validators;
using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, Banner?>
{
  private readonly IBannerQuerier _bannerQuerier;
  private readonly IBannerRepository _bannerRepository;

  public UpdateBannerCommandHandler(IBannerQuerier bannerQuerier, IBannerRepository bannerRepository)
  {
    _bannerQuerier = bannerQuerier;
    _bannerRepository = bannerRepository;
  }

  public async Task<Banner?> Handle(UpdateBannerCommand command, CancellationToken cancellationToken)
  {
    UpdateBannerPayload payload = command.Payload;
    new UpdateBannerValidator().ValidateAndThrow(payload);

    BannerAggregate? banner = await _bannerRepository.LoadAsync(command.Id, cancellationToken);
    if (banner == null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      banner.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      banner.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    banner.Update(command.ActorId);

    await _bannerRepository.SaveAsync(banner, cancellationToken);

    return await _bannerQuerier.ReadAsync(banner, cancellationToken);
  }
}
