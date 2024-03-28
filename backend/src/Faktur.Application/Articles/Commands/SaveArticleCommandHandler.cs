using Faktur.Domain.Articles;
using Faktur.Domain.Articles.Events;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal class SaveArticleCommandHandler : IRequestHandler<SaveArticleCommand>
{
  private readonly IArticleRepository _articleRepository;

  public SaveArticleCommandHandler(IArticleRepository articleRepository)
  {
    _articleRepository = articleRepository;
  }

  public async Task Handle(SaveArticleCommand command, CancellationToken cancellationToken)
  {
    ArticleAggregate article = command.Article;

    bool hasGtinChanged = false;
    foreach (DomainEvent change in article.Changes)
    {
      if (change is ArticleUpdatedEvent updated && updated.Gtin != null)
      {
        hasGtinChanged = true;
      }
    }

    if (hasGtinChanged && article.Gtin != null)
    {
      ArticleAggregate? other = await _articleRepository.LoadAsync(article.Gtin, cancellationToken);
      if (other != null && !other.Equals(article))
      {
        throw new GtinAlreadyUsedException(article.Gtin, nameof(article.Gtin));
      }
    }

    await _articleRepository.SaveAsync(article, cancellationToken);
  }
}
