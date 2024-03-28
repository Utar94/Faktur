using Faktur.Application.Articles.Validators;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal class ReplaceArticleCommandHandler : IRequestHandler<ReplaceArticleCommand, Article?>
{
  private readonly IArticleQuerier _articleQuerier;
  private readonly IArticleRepository _articleRepository;
  private readonly ISender _sender;

  public ReplaceArticleCommandHandler(IArticleQuerier articleQuerier, IArticleRepository articleRepository, ISender sender)
  {
    _articleQuerier = articleQuerier;
    _articleRepository = articleRepository;
    _sender = sender;
  }

  public async Task<Article?> Handle(ReplaceArticleCommand command, CancellationToken cancellationToken)
  {
    ReplaceArticlePayload payload = command.Payload;
    new ReplaceArticleValidator().ValidateAndThrow(payload);

    ArticleAggregate? article = await _articleRepository.LoadAsync(command.Id, cancellationToken);
    if (article == null)
    {
      return null;
    }
    ArticleAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _articleRepository.LoadAsync(command.Id, command.Version.Value, cancellationToken);
    }

    GtinUnit? gtin = GtinUnit.TryCreate(payload.Gtin);
    if (reference == null || gtin != reference.Gtin)
    {
      article.Gtin = gtin;
    }
    DisplayNameUnit displayName = new(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      article.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      article.Description = description;
    }

    article.Update(command.ActorId);

    await _sender.Send(new SaveArticleCommand(article), cancellationToken);

    return await _articleQuerier.ReadAsync(article, cancellationToken);
  }
}
