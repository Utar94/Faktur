using Faktur.Domain.Products;
using Faktur.Domain.Products.Events;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class SaveProductCommandHandler : IRequestHandler<SaveProductCommand>
{
  private readonly IProductRepository _productRepository;

  public SaveProductCommandHandler(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task Handle(SaveProductCommand command, CancellationToken cancellationToken)
  {
    ProductAggregate product = command.Product;

    bool hasSkuChanged = false;
    foreach (DomainEvent change in product.Changes)
    {
      if (change is ProductUpdatedEvent updated && updated.Sku != null)
      {
        hasSkuChanged = true;
      }
    }

    if (hasSkuChanged && product.Sku != null)
    {
      ProductAggregate? other = await _productRepository.LoadAsync(product.StoreId, product.Sku, cancellationToken);
      if (other != null && !other.Equals(product))
      {
        throw new SkuAlreadyUsedException(product.StoreId, product.Sku, nameof(product.Sku));
      }
    }

    await _productRepository.SaveAsync(product, cancellationToken);
  }
}
