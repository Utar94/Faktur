using Faktur.Contracts;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateReceiptCommandTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public UpdateReceiptCommandTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    UpdateReceiptPayload payload = new();
    UpdateReceiptCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateReceiptPayload payload = new()
    {
      Number = new Modification<string>(RandomStringGenerator.GetString("0123456789ABCDEF", NumberUnit.MaximumLength + 1))
    };
    UpdateReceiptCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Number.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing receipt.")]
  public async Task It_should_update_an_existing_receipt()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store, actorId: ActorId);
    await _receiptRepository.SaveAsync(receipt);

    UpdateReceiptPayload payload = new()
    {
      Number = new Modification<string>("117011")
    };
    UpdateReceiptCommand command = new(receipt.Id.ToGuid(), payload);
    Receipt? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(receipt.Id.ToGuid(), result.Id);
    Assert.Equal(receipt.Version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.Equal(payload.Number.Value, result.Number);
  }
}
