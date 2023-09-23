using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Queries;

internal class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, SearchResults<Article>>
{
  private readonly IArticleQuerier articleQuerier;

  public SearchArticlesQueryHandler(IArticleQuerier articleQuerier)
  {
    this.articleQuerier = articleQuerier;
  }

  public async Task<SearchResults<Article>> Handle(SearchArticlesQuery query, CancellationToken cancellationToken)
  {
    return await articleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
