using Faktur.Contracts.Departments;
using Faktur.Domain.Departments;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteDepartmentCommandTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public DeleteDepartmentCommandTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Stores.Table];
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

  // TODO(fpion): Remove Product Departments
}
