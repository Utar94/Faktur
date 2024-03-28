using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateOrReplaceDepartmentCommandTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public CreateOrReplaceDepartmentCommandTests() : base()
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

  [Fact(DisplayName = "It should create a new department.")]
  public async Task It_should_create_a_new_department()
  {
    CreateOrReplaceDepartmentPayload payload = new("PRET-A-MANGER");
    CreateOrReplaceDepartmentCommand command = new(_store.Id.ToGuid(), Number: "36", payload, Version: null);
    CreateOrReplaceDepartmentResult? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.True(result.IsCreated);

    Department department = result.Department;
    Assert.Equal(_store.Id.ToGuid(), department.Store.Id);
    Assert.Equal(command.Number, department.Number);
    Assert.Equal(payload.DisplayName, department.DisplayName);
    Assert.Equal(payload.Description, department.Description);

    Assert.Equal(Actor, department.CreatedBy);
    Assert.Equal(Actor, department.UpdatedBy);
    Assert.Equal(department.CreatedOn, department.UpdatedOn);
  }

  [Fact(DisplayName = "It should replace an existing department.")]
  public async Task It_should_replace_an_existing_department()
  {
    NumberUnit number = new("36");
    DepartmentUnit existing = new(new DisplayNameUnit("PRET-A-MANGER"));
    _store.SetDepartment(number, existing, ActorId);
    long version = _store.Version;
    await _storeRepository.SaveAsync(_store);

    DescriptionUnit description = new("Aucune préparation n’est requise pour ces aliments.");
    existing = new(existing.DisplayName, description);
    _store.SetDepartment(number, existing, ActorId);
    await _storeRepository.SaveAsync(_store);

    CreateOrReplaceDepartmentPayload payload = new("PRET-A-MANGER");
    CreateOrReplaceDepartmentCommand command = new(_store.Id.ToGuid(), number.Value, payload, version);
    CreateOrReplaceDepartmentResult? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.False(result.IsCreated);

    Department department = result.Department;
    Assert.Equal(_store.Id.ToGuid(), department.Store.Id);
    Assert.Equal(command.Number, department.Number);
    Assert.Equal(payload.DisplayName, department.DisplayName);
    Assert.Equal(description.Value, department.Description);

    Assert.Equal(Actor, department.CreatedBy);
    Assert.Equal(Actor, department.UpdatedBy);
    Assert.True(department.CreatedOn < department.UpdatedOn);
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    CreateOrReplaceDepartmentPayload payload = new("PRET-A-MANGER");
    CreateOrReplaceDepartmentCommand command = new(StoreId: Guid.NewGuid(), Number: "36", payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the number is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_number_is_not_valid()
  {
    string number = RandomStringGenerator.GetString(NumberUnit.MaximumLength + 1);
    CreateOrReplaceDepartmentPayload payload = new("PRET-A-MANGER");
    CreateOrReplaceDepartmentCommand command = new(StoreId: Guid.NewGuid(), number, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Number", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceDepartmentPayload payload = new();
    CreateOrReplaceDepartmentCommand command = new(StoreId: Guid.NewGuid(), Number: "36", payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
