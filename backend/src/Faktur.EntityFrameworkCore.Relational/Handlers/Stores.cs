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

  public class StoreDepartmentRemovedEventHandler : INotificationHandler<StoreDepartmentRemovedEvent>
  {
    private readonly FakturContext _context;

    public StoreDepartmentRemovedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(StoreDepartmentRemovedEvent @event, CancellationToken cancellationToken)
    {
      StoreEntity store = await _context.Stores
       .Include(x => x.Departments)
       .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
       ?? throw new InvalidOperationException($"The store 'AggregateId={@event.AggregateId}' could not be found.");

      store.RemoveDepartment(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class StoreDepartmentSavedEventHandler : INotificationHandler<StoreDepartmentSavedEvent>
  {
    private readonly FakturContext _context;

    public StoreDepartmentSavedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(StoreDepartmentSavedEvent @event, CancellationToken cancellationToken)
    {
      StoreEntity store = await _context.Stores
        .Include(x => x.Departments)
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The store 'AggregateId={@event.AggregateId}' could not be found.");

      store.SetDepartment(@event);

      await _context.SaveChangesAsync(cancellationToken);
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
      StoreEntity store = await _context.Stores
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The store 'AggregateId={@event.AggregateId}' could not be found.");

      if (@event.BannerId != null)
      {
        BannerEntity? banner = null;
        if (@event.BannerId.Value != null)
        {
          banner = await _context.Banners
            .SingleOrDefaultAsync(x => x.AggregateId == @event.BannerId.Value.Value, cancellationToken)
            ?? throw new InvalidOperationException($"The banner 'AggregateId={@event.BannerId.Value.Value}' could not be found.");
        }
        store.SetBanner(banner);
      }

      store.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
