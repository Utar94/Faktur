using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Stores;

internal class StoreDeletedEventHandler : INotificationHandler<StoreDeletedEvent>
{
  private readonly FakturContext context;

  public StoreDeletedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(StoreDeletedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity? store = await context.Stores
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (store != null)
    {
      context.Stores.Remove(store);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
