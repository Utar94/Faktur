using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Stores;

internal class StoreCreatedEventHandler : INotificationHandler<StoreCreatedEvent>
{
  private readonly FakturContext context;

  public StoreCreatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(StoreCreatedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity? store = await context.Stores.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (store == null)
    {
      store = new(@event);
      context.Stores.Add(store);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
