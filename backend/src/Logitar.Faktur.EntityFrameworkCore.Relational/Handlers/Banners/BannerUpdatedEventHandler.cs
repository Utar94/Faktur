using Logitar.Faktur.Domain.Banners.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Banners;

internal class BannerUpdatedEventHandler : INotificationHandler<BannerUpdatedEvent>
{
  private readonly FakturContext context;

  public BannerUpdatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(BannerUpdatedEvent @event, CancellationToken cancellationToken)
  {
    BannerEntity banner = await context.Banners
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<BannerEntity>(@event.AggregateId);

    banner.Update(@event);

    await context.SaveChangesAsync(cancellationToken);
  }
}
