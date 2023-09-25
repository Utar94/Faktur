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

    BannerEntity? banner = null;
    if (@event.BannerId?.Value != null)
    {
      banner = await context.Banners
        .SingleOrDefaultAsync(x => x.AggregateId == @event.BannerId.Value.Value.Value, cancellationToken)
        ?? throw new EntityNotFoundException<BannerEntity>(@event.BannerId.Value.Value);
    }

    store.Update(@event, banner);

    await context.SaveChangesAsync(cancellationToken);
  }
}
