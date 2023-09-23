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

internal class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IArticleRepository articleRepository;

  public UpdateArticleCommandHandler(IApplicationContext applicationContext, IArticleRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
  {
    UpdateArticlePayload payload = command.Payload;
    new UpdateArticlePayloadValidator().ValidateAndThrow(payload);

    ArticleId id = ArticleId.Parse(command.Id, nameof(command.Id));
    ArticleAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<ArticleAggregate>(id.AggregateId, nameof(command.Id));

    if (payload.Gtin != null)
    {
      article.Gtin = GtinUnit.TryCreate(payload.Gtin.Value); // TODO(fpion): ensure unicity
    }
    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      article.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      article.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    article.Update(applicationContext.ActorId);

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
