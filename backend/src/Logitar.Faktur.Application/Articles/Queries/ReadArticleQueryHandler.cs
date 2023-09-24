using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Domain.Articles;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Queries;

internal class ReadArticleQueryHandler : IRequestHandler<ReadArticleQuery, Article?>
{
  private readonly IArticleQuerier articleQuerier;

  public ReadArticleQueryHandler(IArticleQuerier articleQuerier)
  {
    this.articleQuerier = articleQuerier;
  }

  public async Task<Article?> Handle(ReadArticleQuery query, CancellationToken cancellationToken)
  {
    Dictionary<string, Article> articles = new(capacity: 2);

    if (!string.IsNullOrWhiteSpace(query.Id))
    {
      Article? article = await articleQuerier.ReadAsync(query.Id, cancellationToken);
      if (article != null)
      {
        articles[article.Id] = article;
      }
    }

    GtinUnit? gtin = GtinUnit.TryCreate(query.Gtin);
    if (gtin != null)
    {
      Article? article = await articleQuerier.ReadAsync(gtin, cancellationToken);
      if (article != null)
      {
        articles[article.Id] = article;
      }
    }

    if (articles.Count > 1)
    {
      throw new TooManyResultsException<Article>(expected: 1, actual: articles.Count);
    }

    return articles.Values.SingleOrDefault();
  }
}
