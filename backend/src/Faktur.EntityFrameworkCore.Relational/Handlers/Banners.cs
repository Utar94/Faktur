using Faktur.Domain.Banners.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Banners
{
  public class BannerCreatedEventHandler : INotificationHandler<BannerCreatedEvent>
  {
    private readonly FakturContext _context;

    public BannerCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(BannerCreatedEvent @event, CancellationToken cancellationToken)
    {
      BannerEntity? banner = await _context.Banners.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (banner == null)
      {
        banner = new(@event);

        _context.Banners.Add(banner);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class BannerDeletedEventHandler : INotificationHandler<BannerDeletedEvent>
  {
    private readonly FakturContext _context;

    public BannerDeletedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(BannerDeletedEvent @event, CancellationToken cancellationToken)
    {
      BannerEntity? banner = await _context.Banners
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (banner != null)
      {
        _context.Banners.Remove(banner);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class BannerUpdatedEventHandler : INotificationHandler<BannerUpdatedEvent>
  {
    private readonly FakturContext _context;

    public BannerUpdatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(BannerUpdatedEvent @event, CancellationToken cancellationToken)
    {
      BannerEntity? banner = await _context.Banners
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (banner != null)
      {
        banner.Update(@event);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}
