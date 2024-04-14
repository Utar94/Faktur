using Faktur.Contracts;
using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Products.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateProductCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;
  private readonly StoreAggregate _store;

  public UpdateProductCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the product cannot be found.")]
  public async Task It_should_return_null_when_the_product_cannot_be_found()
  {
    UpdateProductPayload payload = new();
    UpdateProductCommand command = new(Id: Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateProductPayload payload = new()
    {
      Flags = new Modification<string>(RandomStringGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZ", FlagsUnit.MaximumLength + 1))
    };
    UpdateProductCommand command = new(Id: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Flags.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing product.")]
  public async Task It_should_update_an_existing_product()
  {
    ProductAggregate aggregate = new(_store, _article, ActorId)
    {
      DepartmentNumber = new NumberUnit("36"),
      Flags = new FlagsUnit("FPMRJ"),
      UnitPrice = 9.99m
    };
    await _productRepository.SaveAsync(aggregate);

    UpdateProductPayload payload = new()
    {
      Description = new Modification<string>("Un succulent poulet rôti.")
    };
    UpdateProductCommand command = new(aggregate.Id.ToGuid(), payload);
    Product? product = await Mediator.Send(command);
    Assert.NotNull(product);
    Assert.Equal(aggregate.Id.ToGuid(), product.Id);
    Assert.Equal(payload.Description.Value, product.Description);
  }
}
