using Faktur.Contracts.Departments;
using Faktur.Domain.Departments;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadDepartmentQueryTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public ReadDepartmentQueryTests() : base()
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

  [Fact(DisplayName = "It should return null when the department cannot be found.")]
  public async Task It_should_return_null_when_the_department_cannot_be_found()
  {
    ReadDepartmentQuery query = new(_store.Id.ToGuid(), Number: "36");
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the department when it is found.")]
  public async Task It_should_return_the_department_when_it_is_found()
  {
    NumberUnit number = new("36");
    DepartmentUnit department = new(new DisplayNameUnit("PRET-A-MANGER"));
    _store.SetDepartment(number, department);
    await _storeRepository.SaveAsync(_store);

    ReadDepartmentQuery query = new(_store.Id.ToGuid(), number.Value);
    Department? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(number.Value, result.Number);
    Assert.Equal(_store.Id.ToGuid(), result.Store.Id);
  }
}
