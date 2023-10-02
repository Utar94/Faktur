using Logitar.Faktur.Domain.Products.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Products;

internal class ProductSavedEventHandler : INotificationHandler<ProductSavedEvent>
{
  private readonly FakturContext context;

  public ProductSavedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(ProductSavedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity store = await context.Stores
      .Include(x => x.Departments)
      .Include(x => x.Products).ThenInclude(x => x.Article)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<StoreEntity>(@event.AggregateId);

    ArticleEntity article = await context.Articles
      .SingleOrDefaultAsync(x => x.AggregateId == @event.Product.ArticleId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<ArticleEntity>(@event.Product.ArticleId.AggregateId);

    store.SetProduct(@event, article);

    await context.SaveChangesAsync(cancellationToken);
  }
}
