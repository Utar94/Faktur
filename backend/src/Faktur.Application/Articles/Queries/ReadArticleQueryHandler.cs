using Faktur.Contracts.Articles;
using Logitar.Portal.Contracts;
using MediatR;

namespace Faktur.Application.Articles.Queries;

internal class ReadArticleQueryHandler : IRequestHandler<ReadArticleQuery, Article?>
{
  private readonly IArticleQuerier _articleQuerier;

  public ReadArticleQueryHandler(IArticleQuerier articleQuerier)
  {
    _articleQuerier = articleQuerier;
  }

  public async Task<Article?> Handle(ReadArticleQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Article> articles = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Article? article = await _articleQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (article != null)
      {
        articles[article.Id] = article;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Gtin))
    {
      Article? article = await _articleQuerier.ReadAsync(query.Gtin, cancellationToken);
      if (article != null)
      {
        articles[article.Id] = article;
      }
    }

    if (articles.Count > 1)
    {
      throw new TooManyResultsException<Article>(expectedCount: 1, actualCount: articles.Count);
    }

    return articles.Values.SingleOrDefault();
  }
}
