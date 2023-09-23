using Logitar.Faktur.Application.Articles.Commands;
using Logitar.Faktur.Application.Articles.Queries;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Articles;

internal class ArticleService : IArticleService
{
  private readonly IMediator mediator;

  public ArticleService(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public async Task<AcceptedCommand> CreateAsync(CreateArticlePayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new CreateArticleCommand(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new DeleteArticleCommand(id), cancellationToken);
  }

  public async Task<Article?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReadArticleQuery(id), cancellationToken);
  }

  public async Task<AcceptedCommand> ReplaceAsync(string id, ReplaceArticlePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReplaceArticleCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SearchArticlesQuery(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> UpdateAsync(string id, UpdateArticlePayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new UpdateArticleCommand(id, payload), cancellationToken);
  }
}
