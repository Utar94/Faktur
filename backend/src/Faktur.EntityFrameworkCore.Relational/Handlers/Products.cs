using Faktur.Domain.Products.Events;
using Faktur.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Products
{
  public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
  {
    private readonly FakturContext _context;

    public ProductCreatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ProductCreatedEvent @event, CancellationToken cancellationToken)
    {
      ProductEntity? product = await _context.Products.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (product == null)
      {
        StoreEntity store = await _context.Stores
          .SingleOrDefaultAsync(x => x.AggregateId == @event.StoreId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The store 'AggregateId={@event.StoreId.Value}' could not be found.");
        ArticleEntity article = await _context.Articles
          .SingleOrDefaultAsync(x => x.AggregateId == @event.ArticleId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The article 'AggregateId={@event.ArticleId.Value}' could not be found.");

        product = new(store, article, @event);

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
  {
    private readonly FakturContext _context;

    public ProductDeletedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ProductDeletedEvent @event, CancellationToken cancellationToken)
    {
      ProductEntity? product = await _context.Products
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (product != null)
      {
        _context.Products.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
  {
    private readonly FakturContext _context;

    public ProductUpdatedEventHandler(FakturContext context)
    {
      _context = context;
    }

    public async Task Handle(ProductUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ProductEntity product = await _context.Products
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The product 'AggregateId={@event.AggregateId}' could not be found.");

      DepartmentEntity? department = null;
      if (@event.DepartmentNumber?.Value != null)
      {
        department = await _context.Departments
          .SingleOrDefaultAsync(x => x.StoreId == product.StoreId && x.NumberNormalized == @event.DepartmentNumber.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The department 'StoreId={product.StoreId}, Number={@event.DepartmentNumber.Value.Value}' could not be found.");
      }

      product.Update(@event, department);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
