using Faktur.Domain.Receipts.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Receipts
{
  public class ReceiptCalculatedEventHandler : INotificationHandler<ReceiptCalculatedEvent>
  {
    private readonly FakturContext _context;

    public ReceiptCalculatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ReceiptCalculatedEvent @event, CancellationToken cancellationToken)
    {
      ReceiptEntity receipt = await _context.Receipts
        .Include(x => x.Items)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The receipt 'AggregateId={@event.AggregateId}' could not be found.");

      receipt.Calculate(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class ReceiptCategorizedEventHandler : INotificationHandler<ReceiptCategorizedEvent>
  {
    private readonly FakturContext _context;

    public ReceiptCategorizedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ReceiptCategorizedEvent @event, CancellationToken cancellationToken)
    {
      ReceiptEntity receipt = await _context.Receipts
        .Include(x => x.Items)
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The receipt 'AggregateId={@event.AggregateId}' could not be found.");

      receipt.Categorize(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

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

    public class ReceiptItemChangedEventHandler : INotificationHandler<ReceiptItemChangedEvent>
    {
      private readonly FakturContext _context;

      public ReceiptItemChangedEventHandler(FakturContext context)
      {
        _context = context;
      }

      public async Task Handle(ReceiptItemChangedEvent @event, CancellationToken cancellationToken)
      {
        ReceiptEntity receipt = await _context.Receipts
          .Include(x => x.Items)
          .Include(x => x.Taxes)
          .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The receipt 'AggregateId={@event.AggregateId}' could not be found.");

        ProductEntity? product = null;
        if (@event.Item.Sku != null)
        {
          string skuNormalized = @event.Item.Sku.Value.ToUpper();
          product = await _context.Products
            .SingleOrDefaultAsync(x => x.StoreId == receipt.StoreId && x.SkuNormalized == skuNormalized, cancellationToken);
        }
        else if (@event.Item.Gtin != null)
        {
          product = await _context.Products
            .Include(x => x.Article)
            .SingleOrDefaultAsync(x => x.StoreId == receipt.StoreId && x.Article!.GtinNormalized == @event.Item.Gtin.NormalizedValue, cancellationToken);
        }

        receipt.SetItem(product, @event);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }

    public class ReceiptItemRemovedEventHandler : INotificationHandler<ReceiptItemRemovedEvent>
    {
      private readonly FakturContext _context;

      public ReceiptItemRemovedEventHandler(FakturContext context)
      {
        _context = context;
      }

      public async Task Handle(ReceiptItemRemovedEvent @event, CancellationToken cancellationToken)
      {
        ReceiptEntity receipt = await _context.Receipts
          .Include(x => x.Items)
          .Include(x => x.Taxes)
          .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The receipt 'AggregateId={@event.AggregateId}' could not be found.");

        receipt.RemoveItem(@event);

        await _context.SaveChangesAsync(cancellationToken);
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
