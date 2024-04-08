using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;

  public DeleteArticleCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _article = new(new DisplayNameUnit("PC POULET BBQ"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _articleRepository.SaveAsync(_article);
  }

  [Fact(DisplayName = "It should delete an existing article.")]
  public async Task It_should_delete_an_existing_article()
  {
    DeleteArticleCommand command = new(_article.Id.ToGuid());
    Article? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(_article.Id.ToGuid(), result.Id);

    ArticleEntity? entity = await FakturContext.Articles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == _article.Id.Value);
    Assert.Null(entity);
  }

  [Fact(DisplayName = "It should delete the article products.")]
  public async Task It_should_delete_the_article_products()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    await _storeRepository.SaveAsync(store);

    ProductAggregate product = new(store, _article);
    await _productRepository.SaveAsync(product);

    DeleteArticleCommand command = new(_article.Id.ToGuid());
    _ = await Mediator.Send(command);

    Assert.Empty(await _productRepository.LoadAsync());
    Assert.Empty(await FakturContext.Products.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the article cannot be found.")]
  public async Task It_should_return_null_when_the_article_cannot_be_found()
  {
    DeleteArticleCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
