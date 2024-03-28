using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceReceiptCommandTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public ReplaceReceiptCommandTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should replace an existing receipt.")]
  public async Task It_should_replace_an_existing_receipt()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store, actorId: ActorId);
    long version = receipt.Version;
    await _receiptRepository.SaveAsync(receipt);

    NumberUnit number = new("117011");
    receipt.Number = number;
    receipt.Update(ActorId);
    await _receiptRepository.SaveAsync(receipt);

    ReplaceReceiptPayload payload = new()
    {
      IssuedOn = receipt.IssuedOn,
      Number = null
    };
    ReplaceReceiptCommand command = new(receipt.Id.ToGuid(), payload, version);
    Receipt? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(receipt.Id.ToGuid(), result.Id);
    Assert.Equal(version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.Equal(payload.IssuedOn.ToUniversalTime(), result.IssuedOn);
    Assert.Equal(number.Value, result.Number);
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    ReplaceReceiptPayload payload = new();
    ReplaceReceiptCommand command = new(Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceReceiptPayload payload = new()
    {
      Number = RandomStringGenerator.GetString("0123456789ABCDEF", NumberUnit.MaximumLength + 1)
    };
    ReplaceReceiptCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Number", error.PropertyName);
  }
}
