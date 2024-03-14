using Faktur.Domain.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class DeleteStoreProductsCommandHandler : INotificationHandler<DeleteStoreProductsCommand>
{
  private readonly IProductRepository _productRepository;

  public DeleteStoreProductsCommandHandler(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task Handle(DeleteStoreProductsCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ProductAggregate> products = await _productRepository.LoadAsync(command.Store, cancellationToken);
    foreach (ProductAggregate product in products)
    {
      product.Delete(command.ActorId);
    }
    await _productRepository.SaveAsync(products, cancellationToken);
  }
}
