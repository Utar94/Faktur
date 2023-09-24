using Logitar.Faktur.Domain.Banners.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Banners;

internal class BannerCreatedEventHandler : INotificationHandler<BannerCreatedEvent>
{
  private readonly FakturContext context;

  public BannerCreatedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(BannerCreatedEvent @event, CancellationToken cancellationToken)
  {
    BannerEntity? banner = await context.Banners.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (banner == null)
    {
      banner = new(@event);
      context.Banners.Add(banner);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
