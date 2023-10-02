using Logitar.Faktur.Domain.Products.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Products;

internal class ProductRemovedEventHandler : INotificationHandler<ProductRemovedEvent>
{
  private readonly FakturContext context;

  public ProductRemovedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(ProductRemovedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity store = await context.Stores
      .Include(x => x.Products).ThenInclude(x => x.Article)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<StoreEntity>(@event.AggregateId);

    store.RemoveProduct(@event);

    await context.SaveChangesAsync(cancellationToken);
  }
}
