﻿using Faktur.Domain.Receipts.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Receipts
{
  public class ReceiptCreatedEventHandler : INotificationHandler<ReceiptCreatedEvent>
  {
    private readonly FakturContext _context;

    public ReceiptCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ReceiptCreatedEvent @event, CancellationToken cancellationToken)
    {
      ReceiptEntity? receipt = await _context.Receipts.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (receipt == null)
      {
        StoreEntity store = await _context.Stores
          .SingleOrDefaultAsync(x => x.AggregateId == @event.StoreId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The store 'AggregateId={@event.StoreId.Value}' could not be found.");

        receipt = new(store, @event);

        _context.Receipts.Add(receipt);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }

    public class ReceiptDeletedEventHandler : INotificationHandler<ReceiptDeletedEvent>
    {
      private readonly FakturContext _context;

      public ReceiptDeletedEventHandler(FakturContext context)
      {
        _context = context;
      }

      public async Task Handle(ReceiptDeletedEvent @event, CancellationToken cancellationToken)
      {
        ReceiptEntity? receipt = await _context.Receipts
          .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
        if (receipt != null)
        {
          _context.Receipts.Remove(receipt);

          await _context.SaveChangesAsync(cancellationToken);
        }
      }
    }

    public class ReceiptUpdatedEventHandler : INotificationHandler<ReceiptUpdatedEvent>
    {
      private readonly FakturContext _context;

      public ReceiptUpdatedEventHandler(FakturContext context)
      {
        _context = context;
      }

      public async Task Handle(ReceiptUpdatedEvent @event, CancellationToken cancellationToken)
      {
        ReceiptEntity receipt = await _context.Receipts
          .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The receipt 'AggregateId={@event.AggregateId}' could not be found.");

        receipt.Update(@event);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}