using Logitar.Faktur.Domain.Articles.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Articles;

internal class ArticleDeletedEventHandler : INotificationHandler<ArticleDeletedEvent>
{
  private readonly FakturContext context;

  public ArticleDeletedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(ArticleDeletedEvent @event, CancellationToken cancellationToken)
  {
    ArticleEntity? article = await context.Articles
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (article != null)
    {
      context.Articles.Remove(article);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
