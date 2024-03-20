using Faktur.Application.Articles;
using Faktur.Application.Departments;
using Faktur.Application.Stores;
using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Products.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateOrReplaceProductCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;
  private readonly StoreAggregate _store;

  public CreateOrReplaceProductCommandTests() : base()
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

    TableId[] tables = [FakturDb.Products.Table, FakturDb.Stores.Table, FakturDb.Articles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _articleRepository.SaveAsync(_article);
    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should create a new product.")]
  public async Task It_should_create_a_new_product()
  {
    CreateOrReplaceProductPayload payload = new()
    {
      DepartmentNumber = "36",
      Flags = "FPMRJ",
      UnitPrice = 9.99m
    };
    CreateOrReplaceProductCommand command = new(_store.Id.ToGuid(), _article.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceProductResult result = await Mediator.Send(command);
    Assert.True(result.IsCreated);

    Product product = result.Product;
    Assert.Equal(payload.Sku, product.Sku);
    Assert.Equal(payload.DisplayName, product.DisplayName);
    Assert.Equal(payload.Description, product.Description);
    Assert.Equal(payload.Flags, product.Flags);
    Assert.Equal(payload.UnitPrice, product.UnitPrice);
    Assert.Equal(payload.UnitType, product.UnitType);

    Assert.Equal(_article.Id.ToGuid(), product.Article.Id);
    Assert.Equal(_store.Id.ToGuid(), product.Store.Id);
    Assert.NotNull(product.Department);
    Assert.Equal("36", product.Department.Number);

    Assert.Equal(2, product.Version);
    Assert.Equal(Actor, product.CreatedBy);
    Assert.Equal(Actor, product.UpdatedBy);
  }

  [Fact(DisplayName = "It should replace an existing product.")]
  public async Task It_should_replace_an_existing_product()
  {
    ProductAggregate aggregate = new(_store, _article, ActorId);
    long version = aggregate.Version;
    await _productRepository.SaveAsync(aggregate);

    DescriptionUnit description = new("Un succulent poulet rôti.");
    aggregate.Description = description;
    aggregate.Update(ActorId);
    await _productRepository.SaveAsync(aggregate);

    CreateOrReplaceProductPayload payload = new()
    {
      DepartmentNumber = "36",
      Flags = "FPMRJ",
      UnitPrice = 9.99m
    };
    CreateOrReplaceProductCommand command = new(_store.Id.ToGuid(), _article.Id.ToGuid(), payload, version);
    CreateOrReplaceProductResult result = await Mediator.Send(command);
    Assert.False(result.IsCreated);

    Product product = result.Product;
    Assert.Equal(payload.Sku, product.Sku);
    Assert.Equal(payload.DisplayName, product.DisplayName);
    Assert.Equal(description.Value, product.Description);
    Assert.Equal(payload.Flags, product.Flags);
    Assert.Equal(payload.UnitPrice, product.UnitPrice);
    Assert.Equal(payload.UnitType, product.UnitType);

    Assert.Equal(_article.Id.ToGuid(), product.Article.Id);
    Assert.Equal(_store.Id.ToGuid(), product.Store.Id);
    Assert.NotNull(product.Department);
    Assert.Equal("36", product.Department.Number);

    Assert.True(product.Version > version);
    Assert.Equal(Actor, product.CreatedBy);
    Assert.Equal(Actor, product.UpdatedBy);
    Assert.True(product.CreatedOn < product.UpdatedOn);
  }

  [Fact(DisplayName = "It should throw ArticleNotFoundException when the article cannot be found.")]
  public async Task It_should_throw_ArticleNotFoundException_when_the_article_cannot_be_found()
  {
    CreateOrReplaceProductPayload payload = new();
    CreateOrReplaceProductCommand command = new(_store.Id.ToGuid(), ArticleId: Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<ArticleNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(command.ArticleId, exception.ArticleId);
    Assert.Equal("ArticleId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw DepartmentNotFoundException when the department cannot be found.")]
  public async Task It_should_throw_DepartmentNotFoundException_when_the_department_cannot_be_found()
  {
    CreateOrReplaceProductPayload payload = new()
    {
      DepartmentNumber = "21"
    };
    CreateOrReplaceProductCommand command = new(_store.Id.ToGuid(), _article.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<DepartmentNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.DepartmentNumber, exception.DepartmentNumber);
    Assert.Equal("DepartmentNumber", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw StoreNotFoundException when the store cannot be found.")]
  public async Task It_should_throw_StoreNotFoundException_when_the_store_cannot_be_found()
  {
    CreateOrReplaceProductPayload payload = new();
    CreateOrReplaceProductCommand command = new(StoreId: Guid.NewGuid(), _article.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<StoreNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(command.StoreId, exception.StoreId);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceProductPayload payload = new()
    {
      UnitPrice = -9.99m
    };
    CreateOrReplaceProductCommand command = new(_store.Id.ToGuid(), _article.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("UnitPrice.Value", error.PropertyName);
  }
}
