using Faktur.Contracts;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateReceiptItemCommandTests : IntegrationTests
{
  private const ushort ItemNumber = 1;

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

  public UpdateReceiptItemCommandTests() : base()
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

    _store = new StoreAggregate(new DisplayNameUnit("Maxi Drummondville"), ActorId);
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
    _receipt.SetItem(ItemNumber, new ReceiptItemUnit(gtin: null, new SkuUnit("test"), new DisplayNameUnit("Test"), flags: null,
      quantity: 1.0d, unitPrice: 0.99m, price: 0.99m, departmentNumber: null, department: null), ActorId);
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

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    UpdateReceiptItemPayload payload = new();
    UpdateReceiptItemCommand command = new(ReceiptId: Guid.NewGuid(), ItemNumber, payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should return null when the receipt item cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_item_cannot_be_found()
  {
    UpdateReceiptItemPayload payload = new();
    UpdateReceiptItemCommand command = new(_receipt.Id.ToGuid(), ItemNumber: 0, payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateReceiptItemPayload payload = new()
    {
      Quantity = -10d
    };
    UpdateReceiptItemCommand command = new(ReceiptId: Guid.NewGuid(), ItemNumber: 0, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("Quantity.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing receipt item with a GTIN.")]
  public async Task It_should_update_an_existing_receipt_item_with_a_Gtin()
  {
    Assert.NotNull(_chicken.Gtin);
    Assert.NotNull(_chickenProduct.Flags);

    UpdateReceiptItemPayload payload = new()
    {
      GtinOrSku = _chicken.Gtin.Value,
      Label = _chicken.DisplayName.Value,
      Flags = new Modification<string>(_chickenProduct.Flags.Value),
      UnitPrice = 9.99m,
      Price = 9.99m,
      Department = new Modification<DepartmentPayload>(new DepartmentPayload("36", "PRET-A-MANGER"))
    };
    UpdateReceiptItemCommand command = new(_receipt.Id.ToGuid(), ItemNumber, payload);
    ReceiptItem? item = await Mediator.Send(command);
    Assert.NotNull(item);

    Assert.Equal(ItemNumber, item.Number);
    Assert.Equal(payload.GtinOrSku, item.Gtin);
    Assert.Null(item.Sku);
    Assert.Equal(payload.Label, item.Label);
    Assert.Equal(payload.Flags.Value, item.Flags);
    Assert.Equal(1.0d, item.Quantity);
    Assert.Equal(payload.UnitPrice, item.UnitPrice);
    Assert.Equal(payload.Price, item.Price);
    Assert.NotNull(item.Department);
    Assert.NotNull(payload.Department.Value);
    Assert.Equal(payload.Department.Value.Number, item.Department.Number);
    Assert.Equal(payload.Department.Value.DisplayName, item.Department.DisplayName);
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
      .SingleOrDefaultAsync(x => x.Receipt!.AggregateId == _receipt.Id.Value && x.Number == ItemNumber);
    Assert.NotNull(entity);
    Assert.NotNull(entity.Product);
    Assert.Equal(_chickenProduct.Id.Value, entity.Product.AggregateId);
  }

  [Fact(DisplayName = "It should update an existing receipt item with a SKU.")]
  public async Task It_should_update_an_existing_receipt_item_with_a_Sku()
  {
    Assert.NotNull(_bananasProduct.Sku);
    Assert.NotNull(_bananasProduct.Flags);

    UpdateReceiptItemPayload payload = new()
    {
      GtinOrSku = _bananasProduct.Sku.Value,
      Label = _chicken.DisplayName.Value,
      Flags = new Modification<string>(_bananasProduct.Flags.Value),
      Quantity = 1.150d,
      UnitPrice = 1.52m,
      Price = 1.75m,
      Department = new Modification<DepartmentPayload>(new DepartmentPayload("27", "FRUITS ET LEGUMES"))
    };
    UpdateReceiptItemCommand command = new(_receipt.Id.ToGuid(), ItemNumber, payload);
    ReceiptItem? item = await Mediator.Send(command);
    Assert.NotNull(item);

    Assert.Equal(ItemNumber, item.Number);
    Assert.Null(item.Gtin);
    Assert.Equal(payload.GtinOrSku, item.Sku);
    Assert.Equal(payload.Label, item.Label);
    Assert.Equal(payload.Flags.Value, item.Flags);
    Assert.Equal(payload.Quantity, item.Quantity);
    Assert.Equal(payload.UnitPrice, item.UnitPrice);
    Assert.Equal(payload.Price, item.Price);
    Assert.NotNull(item.Department);
    Assert.NotNull(payload.Department.Value);
    Assert.Equal(payload.Department.Value.Number, item.Department.Number);
    Assert.Equal(payload.Department.Value.DisplayName, item.Department.DisplayName);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);
    Assert.Equal(_receipt.Id.ToGuid(), item.Receipt.Id);

    Assert.Equal(1.75m, item.Receipt.SubTotal);
    Assert.Empty(item.Receipt.Taxes);
    Assert.Equal(1.75m, item.Receipt.Total);

    ReceiptItemEntity? entity = await FakturContext.ReceiptItems.AsNoTracking()
      .Include(x => x.Product)
      .Include(x => x.Receipt)
      .SingleOrDefaultAsync(x => x.Receipt!.AggregateId == _receipt.Id.Value && x.Number == ItemNumber);
    Assert.NotNull(entity);
    Assert.NotNull(entity.Product);
    Assert.Equal(_bananasProduct.Id.Value, entity.Product.AggregateId);
  }
}
