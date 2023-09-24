using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Stores;

internal class StoreUpdatedEventHandler : INotificationHandler<StoreUpdatedEvent>
{
  private readonly FakturContext context;

  public StoreUpdatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(StoreUpdatedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity store = await context.Stores
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<StoreEntity>(@event.AggregateId);

    store.Update(@event);

    await context.SaveChangesAsync(cancellationToken);
  }
}
