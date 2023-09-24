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
  private readonly IBannerRepository articleRepository;

  public ReplaceBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(ReplaceBannerCommand command, CancellationToken cancellationToken)
  {
    ReplaceBannerPayload payload = command.Payload;
    new ReplaceBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    BannerAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await articleRepository.LoadAsync(article.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || (payload.DisplayName.Trim() != reference.DisplayName.Value))
    {
      article.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description?.Value))
    {
      article.Description = DescriptionUnit.TryCreate(payload.Description);
    }

    article.Update(applicationContext.ActorId);

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
