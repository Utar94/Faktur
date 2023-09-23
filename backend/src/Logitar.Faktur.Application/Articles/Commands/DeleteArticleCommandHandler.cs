using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Articles;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Commands;

internal class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IArticleRepository articleRepository;

  public DeleteArticleCommandHandler(IApplicationContext applicationContext, IArticleRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(DeleteArticleCommand command, CancellationToken cancellationToken)
  {
    ArticleId id = ArticleId.Parse(command.Id, nameof(command.Id));
    ArticleAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<ArticleAggregate>(id.AggregateId, nameof(command.Id));

    article.Delete(applicationContext.ActorId);
    // TODO(fpion): delete products referencing this article

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
