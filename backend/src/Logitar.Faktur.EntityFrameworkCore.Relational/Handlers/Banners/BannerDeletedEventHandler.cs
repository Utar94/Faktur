using Logitar.Faktur.Domain.Banners.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Banners;

internal class BannerDeletedEventHandler : INotificationHandler<BannerDeletedEvent>
{
  private readonly FakturContext context;

  public BannerDeletedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(BannerDeletedEvent @event, CancellationToken cancellationToken)
  {
    BannerEntity? banner = await context.Banners
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (banner != null)
    {
      context.Banners.Remove(banner);

      await context.SaveChangesAsync(cancellationToken);
    }
  }
}
