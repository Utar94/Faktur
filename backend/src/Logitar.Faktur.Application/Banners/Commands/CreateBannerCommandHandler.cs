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

internal class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;

  public CreateBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
  }

  public async Task<AcceptedCommand> Handle(CreateBannerCommand command, CancellationToken cancellationToken)
  {
    CreateBannerPayload payload = command.Payload;
    new CreateBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId? id = null;
    if (!string.IsNullOrWhiteSpace(payload.Id))
    {
      id = BannerId.Parse(payload.Id, nameof(payload.Id));
      if (await bannerRepository.LoadAsync(id, cancellationToken) != null)
      {
        throw new IdentifierAlreadyUsedException<BannerAggregate>(id.AggregateId, nameof(payload.Id));
      }
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    BannerAggregate banner = new(displayName, applicationContext.ActorId, id)
    {
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    banner.Update(applicationContext.ActorId);

    await bannerRepository.SaveAsync(banner, cancellationToken);

    return applicationContext.AcceptCommand(banner);
  }
}
