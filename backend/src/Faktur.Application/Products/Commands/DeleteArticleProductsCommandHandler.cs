using Faktur.Domain.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class DeleteArticleProductsCommandHandler : INotificationHandler<DeleteArticleProductsCommand>
{
  private readonly IProductRepository _productRepository;

  public DeleteArticleProductsCommandHandler(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task Handle(DeleteArticleProductsCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ProductAggregate> products = await _productRepository.LoadAsync(command.Article, cancellationToken);
    foreach (ProductAggregate product in products)
    {
      product.Delete(command.ActorId);
    }
    await _productRepository.SaveAsync(products, cancellationToken);
  }
}
