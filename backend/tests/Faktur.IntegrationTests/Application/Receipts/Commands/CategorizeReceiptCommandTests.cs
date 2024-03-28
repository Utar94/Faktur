using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CategorizeReceiptCommandTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;
  private readonly ReceiptAggregate _receipt;

  public CategorizeReceiptCommandTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    _receipt = new(_store, actorId: ActorId);
    _receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null), ActorId);
    _receipt.SetItem(2, new ReceiptItemUnit(new GtinUnit("4011"), sku: null, new DisplayNameUnit("BANANES"), new FlagsUnit("MRJ"),
      quantity: 1.150d, unitPrice: 1.52m, price: 1.75m, departmentNumber: null, department: null), ActorId);
    _receipt.SetItem(3, new ReceiptItemUnit(new GtinUnit("62855308015"), sku: null, new DisplayNameUnit("LB MICHE TRANCH"), new FlagsUnit("MRJ"),
      quantity: 2.0d, unitPrice: 4.00m, price: 8.00m, departmentNumber: null, department: null), ActorId);
    _receipt.SetItem(4, new ReceiptItemUnit(new GtinUnit("05526622290"), sku: null, new DisplayNameUnit("CANTON BOUILLON"), new FlagsUnit("MRJ"),
      quantity: 1.0d, unitPrice: 4.79m, price: 4.79m, departmentNumber: null, department: null), ActorId);
    _receipt.Categorize([new KeyValuePair<ushort, CategoryUnit?>(1, new CategoryUnit("Francis"))]);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _storeRepository.SaveAsync(_store);
    await _receiptRepository.SaveAsync(_receipt);
  }

  [Fact(DisplayName = "It should categorize the receipt items.")]
  public async Task It_should_categorize_the_receipt_items()
  {
    CategorizeReceiptPayload payload = new();
    payload.ItemCategories.Add(new ReceiptItemCategory(1, null));
    payload.ItemCategories.Add(new ReceiptItemCategory(2, "Audrey"));
    payload.ItemCategories.Add(new ReceiptItemCategory(3, "Francis"));
    CategorizeReceiptCommand command = new(_receipt.Id.ToGuid(), payload);
    Receipt? receipt = await Mediator.Send(command);
    Assert.NotNull(receipt);

    Assert.True(receipt.HasBeenProcessed);
    Assert.Equal(Actor, receipt.ProcessedBy);
    Assert.NotNull(receipt.ProcessedOn);

    Assert.Equal(4, receipt.Items.Count);
    foreach (ReceiptItem item in receipt.Items)
    {
      switch (item.Number)
      {
        case 2:
          Assert.Equal("Audrey", item.Category);
          break;
        case 3:
          Assert.Equal("Francis", item.Category);
          break;
        default:
          Assert.Null(item.Category);
          break;
      }
    }
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    CategorizeReceiptPayload payload = new();
    CategorizeReceiptCommand command = new(Id: Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CategorizeReceiptPayload payload = new();
    payload.ItemCategories.Add(new ReceiptItemCategory
    {
      Number = 1,
      Category = RandomStringGenerator.GetString(CategoryUnit.MaximumLength + 1)
    });
    CategorizeReceiptCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("ItemCategories[0].Category", error.PropertyName);
  }
}
