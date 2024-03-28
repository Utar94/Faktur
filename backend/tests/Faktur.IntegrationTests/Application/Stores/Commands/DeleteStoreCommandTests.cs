using Faktur.Contracts.Stores;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteStoreCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public DeleteStoreCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should delete an existing store.")]
  public async Task It_should_delete_an_existing_store()
  {
    DeleteStoreCommand command = new(_store.Id.ToGuid());
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(_store.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Stores.ToArrayAsync());

    StoreEntity? entity = await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == _store.Id.Value);
    Assert.Null(entity);
  }

  [Fact(DisplayName = "It should delete the store products.")]
  public async Task It_should_delete_the_store_products()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync(article);

    ProductAggregate product = new(_store, article);
    await _productRepository.SaveAsync(product);

    DeleteStoreCommand command = new(_store.Id.ToGuid());
    _ = await Mediator.Send(command);

    Assert.Empty(await _productRepository.LoadAsync());
    Assert.Empty(await FakturContext.Products.ToArrayAsync());
  }

  [Fact(DisplayName = "It should delete the store receipts.")]
  public async Task It_should_delete_the_store_receipts()
  {
    ReceiptAggregate receipt = new(_store);
    await _receiptRepository.SaveAsync(receipt);

    DeleteStoreCommand command = new(_store.Id.ToGuid());
    _ = await Mediator.Send(command);

    Assert.Empty(await _receiptRepository.LoadAsync());
    Assert.Empty(await FakturContext.Receipts.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    DeleteStoreCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
