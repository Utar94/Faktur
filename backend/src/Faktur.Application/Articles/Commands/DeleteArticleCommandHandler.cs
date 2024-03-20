using Faktur.Application.Products.Commands;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Article?>
{
  private readonly IArticleQuerier _articleQuerier;
  private readonly IArticleRepository _articleRepository;
  private readonly IPublisher _publisher;

  public DeleteArticleCommandHandler(IArticleQuerier articleQuerier, IArticleRepository articleRepository, IPublisher publisher)
  {
    _articleQuerier = articleQuerier;
    _articleRepository = articleRepository;
    _publisher = publisher;
  }

  public async Task<Article?> Handle(DeleteArticleCommand command, CancellationToken cancellationToken)
  {
    ArticleAggregate? article = await _articleRepository.LoadAsync(command.Id, cancellationToken);
    if (article == null)
    {
      return null;
    }
    Article result = await _articleQuerier.ReadAsync(article, cancellationToken);

    article.Delete(command.ActorId);

    await _publisher.Publish(new DeleteArticleProductsCommand(command.ActorId, article), cancellationToken);
    await _articleRepository.SaveAsync(article, cancellationToken);

    return result;
  }
}
