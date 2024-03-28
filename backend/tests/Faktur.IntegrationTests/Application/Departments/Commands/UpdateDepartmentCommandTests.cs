using Faktur.Contracts;
using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Departments.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateDepartmentCommandTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public UpdateDepartmentCommandTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should return null when the department cannot be found.")]
  public async Task It_should_return_null_when_the_department_cannot_be_found()
  {
    UpdateDepartmentPayload payload = new();
    UpdateDepartmentCommand command = new(_store.Id.ToGuid(), Number: "36", payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    UpdateDepartmentPayload payload = new();
    UpdateDepartmentCommand command = new(StoreId: Guid.NewGuid(), Number: "36", payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the number is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_number_is_not_valid()
  {
    UpdateDepartmentPayload payload = new()
    {
      DisplayName = RandomStringGenerator.GetString(DisplayNameUnit.MaximumLength + 1)
    };
    UpdateDepartmentCommand command = new(_store.Id.ToGuid(), Number: "36", payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.DisplayName), error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    string number = RandomStringGenerator.GetString("0123456789", NumberUnit.MaximumLength + 1);
    UpdateDepartmentPayload payload = new();
    UpdateDepartmentCommand command = new(_store.Id.ToGuid(), number, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal(nameof(command.Number), error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing department.")]
  public async Task It_should_update_an_existing_department()
  {
    NumberUnit number = new("36");
    DepartmentUnit department = new(new DisplayNameUnit("PRET-A-MANGER"));
    _store.SetDepartment(number, department);
    await _storeRepository.SaveAsync(_store);

    UpdateDepartmentPayload payload = new()
    {
      Description = new Modification<string>("Aucune préparation n’est requise pour ces aliments.")
    };
    UpdateDepartmentCommand command = new(_store.Id.ToGuid(), number.Value, payload);
    Department? updated = await Mediator.Send(command);
    Assert.NotNull(updated);
    Assert.Equal(number.Value, updated.Number);
    Assert.Equal(_store.Id.ToGuid(), updated.Store.Id);
    Assert.Equal(payload.Description.Value, updated.Description);
  }
}
