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

internal class ReplaceBannerCommandHandler : IRequestHandler<ReplaceBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;

  public ReplaceBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
  }

  public async Task<AcceptedCommand> Handle(ReplaceBannerCommand command, CancellationToken cancellationToken)
  {
    ReplaceBannerPayload payload = command.Payload;
    new ReplaceBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate banner = await bannerRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    BannerAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await bannerRepository.LoadAsync(banner.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || (payload.DisplayName.Trim() != reference.DisplayName.Value))
    {
      banner.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description?.Value))
    {
      banner.Description = DescriptionUnit.TryCreate(payload.Description);
    }

    banner.Update(applicationContext.ActorId);

    await bannerRepository.SaveAsync(banner, cancellationToken);

    return applicationContext.AcceptCommand(banner);
  }
}
