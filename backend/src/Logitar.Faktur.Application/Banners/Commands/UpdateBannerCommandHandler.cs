using FluentValidation;
using Logitar.Faktur.Application.Banners.Validators;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;

  public UpdateBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateBannerCommand command, CancellationToken cancellationToken)
  {
    UpdateBannerPayload payload = command.Payload;
    new UpdateBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate banner = await bannerRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      banner.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      banner.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    banner.Update(applicationContext.ActorId);

    await bannerRepository.SaveAsync(banner, cancellationToken);

    return applicationContext.AcceptCommand(banner);
  }
}
