﻿using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Products.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteProductCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;
  private readonly StoreAggregate _store;

  public DeleteProductCommandTests() : base()
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
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _articleRepository.SaveAsync(_article);
    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should delete an existing product.")]
  public async Task It_should_delete_an_existing_product()
  {
    ProductAggregate aggregate = new(_store, _article, ActorId)
    {
      DepartmentNumber = new NumberUnit("36"),
      Flags = new FlagsUnit("FPMRJ"),
      UnitPrice = 9.99m
    };
    await _productRepository.SaveAsync(aggregate);

    DeleteProductCommand command = new(aggregate.Id.ToGuid());
    Product? product = await Mediator.Send(command);
    Assert.NotNull(product);
    Assert.Equal(aggregate.Id.ToGuid(), product.Id);

    ProductEntity? entity = await FakturContext.Products.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregate.Id.Value);
    Assert.Null(entity);
  }

  [Fact(DisplayName = "It should return null when the product cannot be found.")]
  public async Task It_should_return_null_when_the_product_cannot_be_found()
  {
    DeleteProductCommand command = new(Id: Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
