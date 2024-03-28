using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Products.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadProductQueryTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;
  private readonly StoreAggregate _store;
  private readonly ProductAggregate _product;

  public ReadProductQueryTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _article = new(new DisplayNameUnit("PC POULET BBQ"), ActorId)
    {
      Gtin = new GtinUnit("06038385904")
    };
    _article.Update(ActorId);
    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId)
    {
      Number = new NumberUnit("08872")
    };
    _store.Update(ActorId);
    _store.SetDepartment(new NumberUnit("36"), new DepartmentUnit(new DisplayNameUnit("PRET-A-MANGER")), ActorId);
    _product = new(_store, _article, ActorId)
    {
      DepartmentNumber = new NumberUnit("36"),
      Sku = new SkuUnit("pc-poulet-bbq"),
      Flags = new FlagsUnit("FPMRJ"),
      UnitPrice = 9.99m
    };
    _product.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _articleRepository.SaveAsync(_article);
    await _storeRepository.SaveAsync(_store);
    await _productRepository.SaveAsync(_product);
  }

  [Fact(DisplayName = "It should return null when the product cannot be found.")]
  public async Task It_should_return_null_when_the_product_cannot_be_found()
  {
    ReadProductQuery query = new(Id: Guid.NewGuid(), StoreId: Guid.NewGuid(), ArticleId: Guid.NewGuid(), Sku: "pc-poulet-bbq");
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the product when it is found by ID.")]
  public async Task It_should_return_the_product_when_it_is_found_by_Id()
  {
    ReadProductQuery query = new(_product.Id.ToGuid(), StoreId: null, ArticleId: null, Sku: null);
    Product? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(_product.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should return the product when it is found by store and article IDs.")]
  public async Task It_should_return_the_product_when_it_is_found_by_store_and_article_Ids()
  {
    ReadProductQuery query = new(Id: null, _store.Id.ToGuid(), _article.Id.ToGuid(), Sku: null);
    Product? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(_product.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should return the product when it is found by store ID and SKU.")]
  public async Task It_should_return_the_product_when_it_is_found_by_store_Id_and_Sku()
  {
    Assert.NotNull(_product.Sku);
    ReadProductQuery query = new(Id: null, _store.Id.ToGuid(), ArticleId: null, _product.Sku.Value);
    Product? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(_product.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple products are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_products_are_found()
  {
    ArticleAggregate article = new(new DisplayNameUnit("OASIS JUS ORANG"), ActorId)
    {
      Gtin = new GtinUnit("06731118520")
    };
    article.Update(ActorId);
    await _articleRepository.SaveAsync(article);

    ProductAggregate product = new(_store, article, ActorId)
    {
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 3.99m
    };
    product.Update(ActorId);
    await _productRepository.SaveAsync(product);

    Assert.NotNull(_product.Sku);
    ReadProductQuery query = new(product.Id.ToGuid(), _store.Id.ToGuid(), _article.Id.ToGuid(), _product.Sku.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Product>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
