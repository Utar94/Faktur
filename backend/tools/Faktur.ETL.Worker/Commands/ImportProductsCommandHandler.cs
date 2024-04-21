using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, int>
{
  private readonly IProductRepository _productRepository;

  public ImportProductsCommandHandler(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task<int> Handle(ImportProductsCommand command, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ProductAggregate> products = (await _productRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Id.ToGuid(), x => x);
    int count = 0;
    foreach (Product product in command.Products)
    {
      ProductId id = new(product.Id);

      if (!products.TryGetValue(product.Id, out ProductAggregate? existingProduct))
      {
        ArticleId articleId = new(product.Article.Id);
        ArticleAggregate article = new(articleId.AggregateId);

        StoreId storeId = new(product.Store.Id);
        StoreAggregate store = new(storeId.AggregateId);

        ActorId createdBy = new(product.CreatedBy.Id);
        existingProduct = new(store, article, createdBy, id);
        products[product.Id] = existingProduct;
      }

      existingProduct.DepartmentNumber = product.Department == null ? null : new NumberUnit(product.Department.Number);

      existingProduct.Sku = SkuUnit.TryCreate(product.Sku);
      existingProduct.DisplayName = DisplayNameUnit.TryCreate(product.DisplayName);
      existingProduct.Description = DescriptionUnit.TryCreate(product.Description);

      existingProduct.Flags = FlagsUnit.TryCreate(product.Flags);

      existingProduct.UnitPrice = product.UnitPrice;
      existingProduct.UnitType = product.UnitType;

      ActorId updatedBy = new(product.UpdatedBy.Id);
      existingProduct.Update(updatedBy);

      if (existingProduct.HasChanges)
      {
        count++;
      }
    }

    await _productRepository.SaveAsync(products.Values, cancellationToken);

    return count;
  }
}
