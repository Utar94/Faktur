using Logitar.Faktur.Contracts.Articles;
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
    return await articleQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
