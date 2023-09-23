using Logitar.Faktur.Domain.Articles.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Articles;

internal class ArticleCreatedEventHandler : INotificationHandler<ArticleCreatedEvent>
{
  private readonly FakturContext context;

  public ArticleCreatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(ArticleCreatedEvent @event, CancellationToken cancellationToken)
  {
    ArticleEntity? article = await context.Articles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (article == null)
    {
      article = new(@event);
      context.Articles.Add(article);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
