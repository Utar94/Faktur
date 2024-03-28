using Faktur.Contracts.Departments;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteDepartmentCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public DeleteDepartmentCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"));
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

    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should delete an existing department.")]
  public async Task It_should_delete_an_existing_department()
  {
    NumberUnit number = new("36");
    DepartmentUnit department = new(new DisplayNameUnit("PRET-A-MANGER"));
    _store.SetDepartment(number, department);
    await _storeRepository.SaveAsync(_store);

    DeleteDepartmentCommand command = new(_store.Id.ToGuid(), number.Value);
    Department? deleted = await Mediator.Send(command);
    Assert.NotNull(deleted);
    Assert.Equal(number.Value, deleted.Number);
    Assert.Equal(_store.Id.ToGuid(), deleted.Store.Id);

    Assert.Empty(await FakturContext.Departments.ToArrayAsync());
  }

  [Fact(DisplayName = "It should remove the product departments.")]
  public async Task It_should_remove_the_product_departments()
  {
    NumberUnit departmentNumber = new("36");
    _store.SetDepartment(departmentNumber, new DepartmentUnit(new DisplayNameUnit("PRET-A-MANGER")));
    await _storeRepository.SaveAsync(_store);

    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync(article);

    ProductAggregate product = new(_store, article)
    {
      DepartmentNumber = departmentNumber
    };
    product.Update();
    await _productRepository.SaveAsync(product);

    DeleteDepartmentCommand command = new(_store.Id.ToGuid(), departmentNumber.Value);
    _ = await Mediator.Send(command);

    product = (await _productRepository.LoadAsync(product.Id.ToGuid()))!;
    Assert.NotNull(product);
    Assert.Null(product.Description);
  }

  [Fact(DisplayName = "It should return null when the department cannot be found.")]
  public async Task It_should_return_null_when_the_department_cannot_be_found()
  {
    DeleteDepartmentCommand command = new(_store.Id.ToGuid(), Number: "36");
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    DeleteDepartmentCommand command = new(StoreId: Guid.NewGuid(), Number: "36");
    Assert.Null(await Mediator.Send(command));
  }
}
