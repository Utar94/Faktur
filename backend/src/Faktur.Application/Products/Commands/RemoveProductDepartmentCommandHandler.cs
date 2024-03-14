using Faktur.Domain.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class RemoveProductDepartmentCommandHandler : INotificationHandler<RemoveProductDepartmentCommand>
{
  private readonly IProductRepository _productRepository;

  public RemoveProductDepartmentCommandHandler(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task Handle(RemoveProductDepartmentCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ProductAggregate> products = await _productRepository.LoadAsync(command.Store, command.DepartmentNumber, cancellationToken);
    foreach (ProductAggregate product in products)
    {
      product.DepartmentNumber = null;
      product.Update(command.ActorId);
    }
    await _productRepository.SaveAsync(products, cancellationToken);
  }
}
