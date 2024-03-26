using Faktur.Domain.Taxes.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Taxes
{
  public class TaxCreatedEventHandler : INotificationHandler<TaxCreatedEvent>
  {
    private readonly FakturContext _context;

    public TaxCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(TaxCreatedEvent @event, CancellationToken cancellationToken)
    {
      TaxEntity? tax = await _context.Taxes.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (tax == null)
      {
        tax = new(@event);

        _context.Taxes.Add(tax);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class TaxDeletedEventHandler : INotificationHandler<TaxDeletedEvent>
  {
    private readonly FakturContext _context;

    public TaxDeletedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(TaxDeletedEvent @event, CancellationToken cancellationToken)
    {
      TaxEntity? tax = await _context.Taxes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (tax != null)
      {
        _context.Taxes.Remove(tax);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class TaxUpdatedEventHandler : INotificationHandler<TaxUpdatedEvent>
  {
    private readonly FakturContext _context;

    public TaxUpdatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(TaxUpdatedEvent @event, CancellationToken cancellationToken)
    {
      TaxEntity tax = await _context.Taxes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The tax 'AggregateId={@event.AggregateId}' could not be found.");

      tax.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
