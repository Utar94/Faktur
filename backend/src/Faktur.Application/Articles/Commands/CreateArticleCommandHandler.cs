using Faktur.Application.Articles.Validators;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Article>
{
  private readonly IArticleQuerier _articleQuerier;
  private readonly IPublisher _publisher;

  public CreateArticleCommandHandler(IArticleQuerier articleQuerier, IPublisher publisher)
  {
    _articleQuerier = articleQuerier;
    _publisher = publisher;
  }

  public async Task<Article> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
  {
    CreateArticlePayload payload = command.Payload;
    new CreateArticleValidator().ValidateAndThrow(payload);

    DisplayNameUnit displayName = new(payload.DisplayName);
    ArticleAggregate article = new(displayName, command.ActorId)
    {
      Gtin = GtinUnit.TryCreate(payload.Gtin),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };

    article.Update(command.ActorId);

    await _publisher.Publish(new SaveArticleCommand(article), cancellationToken);

    return await _articleQuerier.ReadAsync(article, cancellationToken);
  }
}
