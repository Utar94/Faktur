using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateReceiptCommandTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  public CreateReceiptCommandTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should create a new receipt.")]
  public async Task It_should_create_a_new_receipt()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    await _storeRepository.SaveAsync(store);

    CreateReceiptPayload payload = new()
    {
      StoreId = store.Id.ToGuid(),
      IssuedOn = DateTime.Now.AddDays(-1),
      Number = "117011"
    };
    CreateReceiptCommand command = new(payload);
    Receipt receipt = await Mediator.Send(command);

    Assert.Equal(1, receipt.Version);
    Assert.Equal(receipt.CreatedOn, receipt.UpdatedOn);
    Assert.Equal(Actor, receipt.CreatedBy);
    Assert.Equal(Actor, receipt.UpdatedBy);

    Assert.Equal(payload.IssuedOn.Value.ToUniversalTime(), receipt.IssuedOn);
    Assert.Equal(payload.Number, receipt.Number);

    Assert.Equal(0, receipt.ItemCount);
    Assert.Empty(receipt.Items);

    Assert.Equal(0, receipt.SubTotal);
    Assert.Empty(receipt.Taxes);
    Assert.Equal(0, receipt.Total);

    Assert.False(receipt.HasBeenProcessed);
    Assert.Null(receipt.ProcessedBy);
    Assert.Null(receipt.ProcessedOn);

    Assert.Equal(store.Id.ToGuid(), receipt.Store.Id);

    ReceiptEntity? entity = await FakturContext.Receipts.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(receipt.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw StoreNotFoundException when the store cannot be found.")]
  public async Task It_should_throw_StoreNotFoundException_when_the_store_cannot_be_found()
  {
    CreateReceiptPayload payload = new()
    {
      StoreId = Guid.NewGuid()
    };
    CreateReceiptCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<StoreNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.StoreId, exception.AggregateId.ToGuid());
    Assert.Equal(nameof(payload.StoreId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateReceiptPayload payload = new()
    {
      IssuedOn = DateTime.Now.AddDays(1)
    };
    CreateReceiptCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("PastValidator", error.ErrorCode);
    Assert.Equal("IssuedOn.Value", error.PropertyName);
  }
}
