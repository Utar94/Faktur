using Faktur.Application.Banners.Validators;
using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, Banner>
{
  private readonly IBannerQuerier _bannerQuerier;
  private readonly IBannerRepository _bannerRepository;

  public CreateBannerCommandHandler(IBannerQuerier bannerQuerier, IBannerRepository bannerRepository)
  {
    _bannerQuerier = bannerQuerier;
    _bannerRepository = bannerRepository;
  }

  public async Task<Banner> Handle(CreateBannerCommand command, CancellationToken cancellationToken)
  {
    CreateBannerPayload payload = command.Payload;
    new CreateBannerValidator().ValidateAndThrow(payload);

    DisplayNameUnit displayName = new(payload.DisplayName);
    BannerAggregate banner = new(displayName, command.ActorId)
    {
      Description = DescriptionUnit.TryCreate(payload.Description)
    };

    banner.Update(command.ActorId);

    await _bannerRepository.SaveAsync(banner, cancellationToken);

    return await _bannerQuerier.ReadAsync(banner, cancellationToken);
  }
}
