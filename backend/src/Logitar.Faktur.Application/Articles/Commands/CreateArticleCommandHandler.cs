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

internal class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IArticleRepository articleRepository;

  public CreateArticleCommandHandler(IApplicationContext applicationContext, IArticleRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
  {
    CreateArticlePayload payload = command.Payload;
    new CreateArticlePayloadValidator().ValidateAndThrow(payload);

    GtinUnit? gtin = GtinUnit.TryCreate(payload.Gtin);
    if (gtin != null && await articleRepository.LoadAsync(gtin, cancellationToken) != null)
    {
      throw new GtinAlreadyUsedException(gtin, nameof(payload.Gtin));
    }

    ArticleId? id = null;
    if (!string.IsNullOrWhiteSpace(payload.Id))
    {
      id = ArticleId.Parse(payload.Id, nameof(payload.Id));
      if (await articleRepository.LoadAsync(id, cancellationToken) != null)
      {
        throw new IdentifierAlreadyUsedException<ArticleAggregate>(id.AggregateId, nameof(payload.Id));
      }
    }
    else if (gtin != null)
    {
      id = new(gtin);
      if (await articleRepository.LoadAsync(id, cancellationToken) != null)
      {
        id = null;
      }
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    ArticleAggregate article = new(displayName, applicationContext.ActorId, id)
    {
      Gtin = gtin,
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    article.Update(applicationContext.ActorId);

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
