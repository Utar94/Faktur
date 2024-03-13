using Faktur.Domain.Stores.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Stores
{
  public class StoreCreatedEventHandler : INotificationHandler<StoreCreatedEvent>
  {
    private readonly FakturContext _context;

    public StoreCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(StoreCreatedEvent @event, CancellationToken cancellationToken)
    {
      StoreEntity? store = await _context.Stores.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (store == null)
      {
        store = new(@event);

        _context.Stores.Add(store);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class StoreDeletedEventHandler : INotificationHandler<StoreDeletedEvent>
  {
    private readonly FakturContext _context;

    public StoreDeletedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(StoreDeletedEvent @event, CancellationToken cancellationToken)
    {
      StoreEntity? store = await _context.Stores
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (store != null)
      {
        _context.Stores.Remove(store);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class StoreUpdatedEventHandler : INotificationHandler<StoreUpdatedEvent>
  {
    private readonly FakturContext _context;

    public StoreUpdatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(StoreUpdatedEvent @event, CancellationToken cancellationToken)
    {
      StoreEntity? store = await _context.Stores
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (store != null)
      {
        if (@event.BannerId != null)
        {
          BannerEntity? banner = null;
          if (@event.BannerId.Value != null)
          {
            banner = await _context.Banners.SingleOrDefaultAsync(x => x.AggregateId == @event.BannerId.Value.Value, cancellationToken);
          }
          store.SetBanner(banner);
        }

        store.Update(@event);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}
