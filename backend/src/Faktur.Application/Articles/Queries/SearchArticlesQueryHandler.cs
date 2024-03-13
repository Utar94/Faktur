using Faktur.Contracts.Articles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Articles.Queries;

internal class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, SearchResults<Article>>
{
  private readonly IArticleQuerier _articleQuerier;

  public SearchArticlesQueryHandler(IArticleQuerier articleQuerier)
  {
    _articleQuerier = articleQuerier;
  }

  public async Task<SearchResults<Article>> Handle(SearchArticlesQuery query, CancellationToken cancellationToken)
  {
    return await _articleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
