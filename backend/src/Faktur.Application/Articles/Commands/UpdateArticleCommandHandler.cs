using Faktur.Application.Articles.Validators;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, Article?>
{
  private readonly IArticleQuerier _articleQuerier;
  private readonly IArticleRepository _articleRepository;
  private readonly ISender _sender;

  public UpdateArticleCommandHandler(IArticleQuerier articleQuerier, IArticleRepository articleRepository, ISender sender)
  {
    _articleQuerier = articleQuerier;
    _articleRepository = articleRepository;
    _sender = sender;
  }

  public async Task<Article?> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
  {
    UpdateArticlePayload payload = command.Payload;
    new UpdateArticleValidator().ValidateAndThrow(payload);

    ArticleAggregate? article = await _articleRepository.LoadAsync(command.Id, cancellationToken);
    if (article == null)
    {
      return null;
    }

    if (payload.Gtin != null)
    {
      article.Gtin = GtinUnit.TryCreate(payload.Gtin.Value);
    }
    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      article.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      article.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    article.Update(command.ActorId);

    await _sender.Send(new SaveArticleCommand(article), cancellationToken);

    return await _articleQuerier.ReadAsync(article, cancellationToken);
  }
}
