using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Product?>
{
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;

  public DeleteProductCommandHandler(IProductQuerier productQuerier, IProductRepository productRepository)
  {
    _productQuerier = productQuerier;
    _productRepository = productRepository;
  }

  public async Task<Product?> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
  {
    ProductAggregate? product = await _productRepository.LoadAsync(command.Id, cancellationToken);
    if (product == null)
    {
      return null;
    }
    Product result = await _productQuerier.ReadAsync(product, cancellationToken);

    product.Delete(command.ActorId);

    await _productRepository.SaveAsync(product, cancellationToken);

    return result;
  }
}
