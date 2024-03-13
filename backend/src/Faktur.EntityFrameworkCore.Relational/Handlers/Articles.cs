using Faktur.Domain.Articles.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Articles
{
  public class ArticleCreatedEventHandler : INotificationHandler<ArticleCreatedEvent>
  {
    private readonly FakturContext _context;

    public ArticleCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ArticleCreatedEvent @event, CancellationToken cancellationToken)
    {
      ArticleEntity? article = await _context.Articles.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (article == null)
      {
        article = new(@event);

        _context.Articles.Add(article);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ArticleDeletedEventHandler : INotificationHandler<ArticleDeletedEvent>
  {
    private readonly FakturContext _context;

    public ArticleDeletedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ArticleDeletedEvent @event, CancellationToken cancellationToken)
    {
      ArticleEntity? article = await _context.Articles
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (article != null)
      {
        _context.Articles.Remove(article);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ArticleUpdatedEventHandler : INotificationHandler<ArticleUpdatedEvent>
  {
    private readonly FakturContext _context;

    public ArticleUpdatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ArticleUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ArticleEntity article = await _context.Articles
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The article 'AggregateId={@event.AggregateId}' could not be found.");

      article.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
