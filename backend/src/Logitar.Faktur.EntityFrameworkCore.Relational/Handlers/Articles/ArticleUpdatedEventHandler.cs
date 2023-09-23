using Logitar.Faktur.Domain.Articles.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Articles;

internal class ArticleUpdatedEventHandler : INotificationHandler<ArticleUpdatedEvent>
{
  private readonly FakturContext context;

  public ArticleUpdatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(ArticleUpdatedEvent @event, CancellationToken cancellationToken)
  {
    ArticleEntity article = await context.Articles
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<ArticleEntity>(@event.AggregateId);

    article.Update(@event);

    await context.SaveChangesAsync(cancellationToken);
  }
}
