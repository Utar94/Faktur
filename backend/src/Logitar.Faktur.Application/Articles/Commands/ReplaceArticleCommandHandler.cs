using FluentValidation;
using Logitar.Faktur.Application.Articles.Validators;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Commands;

internal class ReplaceArticleCommandHandler : IRequestHandler<ReplaceArticleCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IArticleRepository articleRepository;

  public ReplaceArticleCommandHandler(IApplicationContext applicationContext, IArticleRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(ReplaceArticleCommand command, CancellationToken cancellationToken)
  {
    ReplaceArticlePayload payload = command.Payload;
    new ReplaceArticlePayloadValidator().ValidateAndThrow(payload);

    ArticleId id = ArticleId.Parse(command.Id, nameof(command.Id));
    ArticleAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<ArticleAggregate>(id.AggregateId, nameof(command.Id));

    ArticleAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await articleRepository.LoadAsync(article.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || (payload.Gtin?.CleanTrim() != reference.Gtin?.Value))
    {
      article.Gtin = GtinUnit.TryCreate(payload.Gtin);
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
