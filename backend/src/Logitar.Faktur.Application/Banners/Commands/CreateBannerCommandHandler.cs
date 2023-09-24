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
  private readonly IBannerRepository articleRepository;

  public CreateBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(CreateBannerCommand command, CancellationToken cancellationToken)
  {
    CreateBannerPayload payload = command.Payload;
    new CreateBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId? id = null;
    if (!string.IsNullOrWhiteSpace(payload.Id))
    {
      id = BannerId.Parse(payload.Id, nameof(payload.Id));
      if (await articleRepository.LoadAsync(id, cancellationToken) != null)
      {
        throw new IdentifierAlreadyUsedException<BannerAggregate>(id.AggregateId, nameof(payload.Id));
      }
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    BannerAggregate article = new(displayName, applicationContext.ActorId, id)
    {
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    article.Update(applicationContext.ActorId);

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
