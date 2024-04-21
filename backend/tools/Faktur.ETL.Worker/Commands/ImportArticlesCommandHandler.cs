using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportArticlesCommandHandler : IRequestHandler<ImportArticlesCommand, int>
{
  private readonly IArticleRepository _articleRepository;

  public ImportArticlesCommandHandler(IArticleRepository articleRepository)
  {
    _articleRepository = articleRepository;
  }

  public async Task<int> Handle(ImportArticlesCommand command, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ArticleAggregate> articles = (await _articleRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Id.ToGuid(), x => x);
    int count = 0;
    foreach (Article article in command.Articles)
    {
      ArticleId id = new(article.Id);

      DisplayNameUnit displayName = new(article.DisplayName);
      if (articles.TryGetValue(article.Id, out ArticleAggregate? existingArticle))
      {
        existingArticle.DisplayName = displayName;
      }
      else
      {
        ActorId createdBy = new(article.CreatedBy.Id);
        existingArticle = new(displayName, createdBy, id);
        articles[article.Id] = existingArticle;
      }

      existingArticle.Gtin = GtinUnit.TryCreate(article.Gtin);
      existingArticle.Description = DescriptionUnit.TryCreate(article.Description);

      ActorId updatedBy = new(article.UpdatedBy.Id);
      existingArticle.Update(updatedBy);

      if (existingArticle.HasChanges)
      {
        existingArticle.SetDates(article);
        count++;
      }
    }

    await _articleRepository.SaveAsync(articles.Values, cancellationToken);

    return count;
  }
}
