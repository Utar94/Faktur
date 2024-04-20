using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateOrReplaceReceiptItemCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _qst;

  private readonly ArticleAggregate _bananas;
  private readonly ArticleAggregate _chicken;

  private readonly StoreAggregate _store;

  private readonly ProductAggregate _bananasProduct;
  private readonly ProductAggregate _chickenProduct;

  private readonly ReceiptAggregate _receipt;

  public CreateOrReplaceReceiptItemCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05d, ActorId)
    {
      Flags = new FlagsUnit("F")
    };
    _gst.Update(ActorId);
    _qst = new(new TaxCodeUnit("QST"), rate: 0.09975d, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    _qst.Update(ActorId);

    _bananas = new(new DisplayNameUnit("BANANES"), ActorId);
    _chicken = new(new DisplayNameUnit("PC POULET BBQ"), ActorId)
    {
      Gtin = new GtinUnit("06038385904")
    };
    _chicken.Update(ActorId);

    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
    _store.SetDepartment(new NumberUnit("27"), new DepartmentUnit(new DisplayNameUnit("FRUITS ET LEGUMES")), ActorId);
    _store.SetDepartment(new NumberUnit("36"), new DepartmentUnit(new DisplayNameUnit("PRET-A-MANGER")), ActorId);

    _bananasProduct = new(_store, _bananas, ActorId)
    {
      DepartmentNumber = new NumberUnit("27"),
      Sku = new SkuUnit("bananes"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 1.52m,
      UnitType = UnitType.Kg
    };
    _bananasProduct.Update(ActorId);
    _chickenProduct = new(_store, _chicken, ActorId)
    {
      DepartmentNumber = new NumberUnit("36"),
      Flags = new FlagsUnit("FPMRJ"),
      UnitPrice = 9.99m
    };
    _chickenProduct.Update(ActorId);

    _receipt = new(_store, actorId: ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync([_gst, _qst]);
    await _articleRepository.SaveAsync([_bananas, _chicken]);
    await _storeRepository.SaveAsync(_store);
    await _productRepository.SaveAsync([_bananasProduct, _chickenProduct]);
    await _receiptRepository.SaveAsync(_receipt);
  }

  [Fact(DisplayName = "It should create a new receipt item.")]
  public async Task It_should_create_a_new_receipt_item()
  {
    Assert.NotNull(_bananasProduct.Sku);
    CreateOrReplaceReceiptItemPayload payload = new(_bananasProduct.Sku.Value, _bananas.DisplayName.Value)
    {
      Quantity = 1.150d,
      UnitPrice = _bananasProduct.UnitPrice,
      Price = 1.75m
    };
    CreateOrReplaceReceiptItemCommand command = new(_receipt.Id.ToGuid(), ItemNumber: 1, payload, Version: null);
    CreateOrReplaceReceiptItemResult? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.True(result.IsCreated);

    ReceiptItem item = result.Item;
    Assert.Equal(command.ItemNumber, item.Number);
    Assert.Null(item.Gtin);
    Assert.Equal(payload.GtinOrSku, item.Sku);
    Assert.Equal(payload.Label, item.Label);
    Assert.Equal(payload.Flags, item.Flags);
    Assert.Equal(payload.Quantity, item.Quantity);
    Assert.Equal(payload.UnitPrice, item.UnitPrice);
    Assert.Equal(payload.Price, item.Price);
    Assert.Null(item.Department);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.Equal(item.CreatedOn, item.UpdatedOn);
    Assert.Equal(_receipt.Id.ToGuid(), item.Receipt.Id);

    Assert.Equal(1.75m, item.Receipt.SubTotal);
    Assert.Empty(item.Receipt.Taxes);
    Assert.Equal(1.75m, item.Receipt.Total);

    ReceiptItemEntity? entity = await FakturContext.ReceiptItems.AsNoTracking()
      .Include(x => x.Product)
      .Include(x => x.Receipt)
      .SingleOrDefaultAsync(x => x.Receipt!.AggregateId == _receipt.Id.Value && x.Number == item.Number);
    Assert.NotNull(entity);
    Assert.NotNull(entity.Product);
    Assert.Equal(_bananasProduct.Id.Value, entity.Product.AggregateId);
  }

  [Fact(DisplayName = "It should replace an existing receipt item.")]
  public async Task It_should_replace_an_existing_receipt_item()
  {
    Assert.NotNull(_chicken.Gtin);
    Assert.NotNull(_chickenProduct.Flags);

    NumberUnit departmentNumber = new("36");
    DepartmentUnit? department = _store.TryFindDepartment(departmentNumber);
    Assert.NotNull(department);

    ushort number = 1;

    _receipt.SetItem(number, new ReceiptItemUnit(_chicken.Gtin, sku: null, _chicken.DisplayName, _chickenProduct.Flags,
      quantity: 1.0d, unitPrice: 8.99m, price: 8.99m, departmentNumber: null, department: null), ActorId);
    await _receiptRepository.SaveAsync(_receipt);

    long version = _receipt.Version;

    _receipt.SetItem(number, new ReceiptItemUnit(_chicken.Gtin, sku: null, _chicken.DisplayName, _chickenProduct.Flags,
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber, department), ActorId);
    await _receiptRepository.SaveAsync(_receipt);

    CreateOrReplaceReceiptItemPayload payload = new(_chicken.Gtin.Value, _chicken.DisplayName.Value)
    {
      Flags = _chickenProduct.Flags.Value,
      Price = 9.99m,
      Department = new DepartmentSummary(departmentNumber.Value, department.DisplayName.Value)
    };
    CreateOrReplaceReceiptItemCommand command = new(_receipt.Id.ToGuid(), number, payload, version);
    CreateOrReplaceReceiptItemResult? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.False(result.IsCreated);

    ReceiptItem item = result.Item;
    Assert.Equal(command.ItemNumber, item.Number);
    Assert.Equal(payload.GtinOrSku, item.Gtin);
    Assert.Null(item.Sku);
    Assert.Equal(payload.Label, item.Label);
    Assert.Equal(payload.Flags, item.Flags);
    Assert.Equal(1.0d, item.Quantity);
    Assert.Equal(payload.Price, item.UnitPrice);
    Assert.Equal(payload.Price, item.Price);
    Assert.NotNull(item.Department);
    Assert.Equal(departmentNumber.Value, item.Department.Number);
    Assert.Equal(department.DisplayName.Value, item.Department.DisplayName);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);
    Assert.Equal(_receipt.Id.ToGuid(), item.Receipt.Id);

    Assert.Equal(9.99m, item.Receipt.SubTotal);
    Assert.Equal(9.99m + 0.50m + 1.00m, item.Receipt.Total);

    Assert.Equal(2, item.Receipt.Taxes.Count);

    ReceiptTax? gst = item.Receipt.Taxes.SingleOrDefault(t => t.Code == _gst.Code.Value);
    Assert.NotNull(gst);
    Assert.Equal(0.05d, gst.Rate);
    Assert.Equal(9.99m, gst.TaxableAmount);
    Assert.Equal(0.50m, gst.Amount);

    ReceiptTax? qst = item.Receipt.Taxes.SingleOrDefault(t => t.Code == _qst.Code.Value);
    Assert.NotNull(qst);
    Assert.Equal(0.09975d, qst.Rate);
    Assert.Equal(9.99m, qst.TaxableAmount);
    Assert.Equal(1.00m, qst.Amount);

    ReceiptItemEntity? entity = await FakturContext.ReceiptItems.AsNoTracking()
      .Include(x => x.Product)
      .Include(x => x.Receipt)
      .SingleOrDefaultAsync(x => x.Receipt!.AggregateId == _receipt.Id.Value && x.Number == item.Number);
    Assert.NotNull(entity);
    Assert.NotNull(entity.Product);
    Assert.Equal(_chickenProduct.Id.Value, entity.Product.AggregateId);
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    CreateOrReplaceReceiptItemPayload payload = new("bananes", "BANANES")
    {
      Price = 1.75m
    };
    CreateOrReplaceReceiptItemCommand command = new(ReceiptId: Guid.NewGuid(), ItemNumber: 0, payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceReceiptItemPayload payload = new("bananes", "BANANES");
    CreateOrReplaceReceiptItemCommand command = new(ReceiptId: Guid.NewGuid(), ItemNumber: 0, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.Price), error.PropertyName);
  }
}
