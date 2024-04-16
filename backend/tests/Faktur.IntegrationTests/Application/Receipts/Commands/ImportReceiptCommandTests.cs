using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ImportReceiptCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _qst;

  private readonly StoreAggregate _store;

  private readonly ArticleAggregate _ananas;
  private readonly ArticleAggregate _bananas;

  private readonly ProductAggregate _ananasProduct;

  public ImportReceiptCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
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

    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);

    _ananas = new(new DisplayNameUnit("ANANAS"), ActorId)
    {
      Gtin = new GtinUnit("4029")
    };
    _ananas.Update(ActorId);
    _bananas = new(new DisplayNameUnit("BANANES"), ActorId)
    {
      Gtin = new GtinUnit("4011")
    };
    _bananas.Update(ActorId);

    _ananasProduct = new(_store, _ananas, ActorId)
    {
      Sku = new SkuUnit("ananas"),
      DisplayName = _ananas.DisplayName,
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 1.67m
    };
    _ananasProduct.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync([_gst, _qst]);
    await _storeRepository.SaveAsync(_store);
    await _articleRepository.SaveAsync([_ananas, _bananas]);
    await _productRepository.SaveAsync(_ananasProduct);
  }

  [Fact(DisplayName = "It should import a new empty receipt.")]
  public async Task It_should_import_a_new_empty_receipt()
  {
    ImportReceiptPayload payload = new()
    {
      StoreId = _store.Id.ToGuid(),
      IssuedOn = DateTime.Now.AddDays(-1),
      Number = "  ",
      Locale = "",
      Lines = "    "
    };
    ImportReceiptCommand command = new(payload);
    Receipt receipt = await Mediator.Send(command);

    Assert.NotEqual(Guid.Empty, receipt.Id);
    Assert.Equal(2, receipt.Version);
    Assert.Equal(Actor, receipt.CreatedBy);
    Assert.Equal(Actor, receipt.UpdatedBy);
    Assert.True(receipt.CreatedOn < receipt.UpdatedOn);

    Assert.Equal(payload.IssuedOn?.ToUniversalTime(), receipt.IssuedOn);
    Assert.Null(receipt.Number);
    Assert.Equal(0, receipt.ItemCount);
    Assert.Empty(receipt.Items);
    Assert.Equal(0, receipt.SubTotal);
    Assert.Equal(0, receipt.Total);
    Assert.Empty(receipt.Taxes);
    Assert.False(receipt.HasBeenProcessed);
    Assert.Null(receipt.ProcessedBy);
    Assert.Null(receipt.ProcessedOn);
    Assert.Equal(_store.Id.ToGuid(), receipt.Store.Id);

    ReceiptEntity? entity = await FakturContext.Receipts.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(receipt.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should import a new receipt.")]
  public async Task It_should_import_a_new_receipt()
  {
    StringBuilder lines = new();
    lines.AppendLine(string.Join('\t', "lays-salt-vinegar-chips", "LAY'S CHIPS", "FPMRJ", "3.75"));
    lines.AppendLine("*27-FRUITS-ET-LEGUMES");
    lines.AppendLine(string.Join('\t', "0000004011", "BANANES", "MRJ", "1.150", "1.52", "1.75"));
    lines.AppendLine(string.Join('\t', "ananas", "ANANAS", "MRJ", "1.67"));

    ImportReceiptPayload payload = new()
    {
      StoreId = _store.Id.ToGuid(),
      IssuedOn = DateTime.Now.AddDays(-1),
      Number = "103599",
      Locale = "en",
      Lines = lines.ToString()
    };
    ImportReceiptCommand command = new(payload);
    Receipt receipt = await Mediator.Send(command);

    Assert.NotEqual(Guid.Empty, receipt.Id);
    Assert.Equal(2, receipt.Version);
    Assert.Equal(Actor, receipt.CreatedBy);
    Assert.Equal(Actor, receipt.UpdatedBy);
    Assert.True(receipt.CreatedOn < receipt.UpdatedOn);

    Assert.Equal(payload.IssuedOn?.ToUniversalTime(), receipt.IssuedOn);
    Assert.Equal(payload.Number, receipt.Number);

    Assert.Equal(3, receipt.ItemCount);
    Assert.Contains(receipt.Items, i => i.Number == 0 && i.Gtin == null && i.Sku == "lays-salt-vinegar-chips" && i.Label == "LAY'S CHIPS" && i.Flags == "FPMRJ"
      && i.Quantity == 1.0d && i.UnitPrice == 3.75m && i.Price == 3.75m && i.Department == null && i.Category == null
      && i.CreatedBy.Equals(Actor) && i.UpdatedBy.Equals(Actor) && i.CreatedOn == i.UpdatedOn);
    DepartmentSummary department = new("27", "FRUITS-ET-LEGUMES");
    Assert.Contains(receipt.Items, i => i.Number == 1 && i.Gtin == "0000004011" && i.Sku == null && i.Label == "BANANES" && i.Flags == "MRJ"
      && i.Quantity == 1.150d && i.UnitPrice == 1.52m && i.Price == 1.75m && i.Department == department && i.Category == null
      && i.CreatedBy.Equals(Actor) && i.UpdatedBy.Equals(Actor) && i.CreatedOn == i.UpdatedOn);
    Assert.Contains(receipt.Items, i => i.Number == 2 && i.Gtin == null && i.Sku == "ananas" && i.Label == "ANANAS" && i.Flags == "MRJ"
      && i.Quantity == 1.0d && i.UnitPrice == 1.67m && i.Price == 1.67m && i.Department == department && i.Category == null
      && i.CreatedBy.Equals(Actor) && i.UpdatedBy.Equals(Actor) && i.CreatedOn == i.UpdatedOn);

    Assert.Equal(7.17m, receipt.SubTotal);
    Assert.Equal(7.73m, receipt.Total);
    Assert.Equal(2, receipt.Taxes.Count);
    Assert.Contains(receipt.Taxes, t => t.Code == _gst.Code.Value && t.Rate == _gst.Rate && t.TaxableAmount == 3.75m && t.Amount == 0.19m);
    Assert.Contains(receipt.Taxes, t => t.Code == _qst.Code.Value && t.Rate == _qst.Rate && t.TaxableAmount == 3.75m && t.Amount == 0.37m);

    Assert.False(receipt.HasBeenProcessed);
    Assert.Null(receipt.ProcessedBy);
    Assert.Null(receipt.ProcessedOn);

    Assert.Equal(_store.Id.ToGuid(), receipt.Store.Id);

    ReceiptEntity? entity = await FakturContext.Receipts.AsNoTracking()
      .Include(x => x.Items)
      .Include(x => x.Store).ThenInclude(x => x!.Departments)
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(receipt.Id).Value);
    Assert.NotNull(entity);
    Assert.All(entity.Items, i => Assert.NotNull(i.ProductId));
    Assert.NotNull(entity.Store);
    Assert.Contains(entity.Store.Departments, d => d.NumberNormalized == department.Number && d.DisplayName == department.DisplayName);

    ProductEntity? bananasProduct = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == _store.Id.Value && x.Article!.GtinNormalized == 4011);
    Assert.NotNull(bananasProduct);
    Assert.NotNull(bananasProduct.Article);
    Assert.Equal(_bananas.Id.Value, bananasProduct.Article.AggregateId);
    Assert.NotNull(bananasProduct.Department);
    Assert.Equal(department.Number, bananasProduct.Department.Number);
    Assert.Null(bananasProduct.Sku);
    Assert.Null(bananasProduct.SkuNormalized);
    Assert.Equal("BANANES", bananasProduct.DisplayName);
    Assert.Null(bananasProduct.Description);
    Assert.Equal("MRJ", bananasProduct.Flags);
    Assert.Equal(1.52m, bananasProduct.UnitPrice);
    Assert.Null(bananasProduct.UnitType);

    ProductEntity? chipsProduct = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == _store.Id.Value && x.SkuNormalized == "LAYS-SALT-VINEGAR-CHIPS");
    Assert.NotNull(chipsProduct);
    Assert.NotNull(chipsProduct.Article);
    Assert.Null(chipsProduct.Article.Gtin);
    Assert.Null(chipsProduct.Article.GtinNormalized);
    Assert.Equal("LAY'S CHIPS", chipsProduct.Article.DisplayName);
    Assert.Null(chipsProduct.Article.Description);
    Assert.Null(chipsProduct.DepartmentId);
    Assert.Equal("lays-salt-vinegar-chips", chipsProduct.Sku);
    Assert.Equal("LAY'S CHIPS", chipsProduct.DisplayName);
    Assert.Null(chipsProduct.Description);
    Assert.Equal("FPMRJ", chipsProduct.Flags);
    Assert.Equal(3.75m, chipsProduct.UnitPrice);
    Assert.Null(chipsProduct.UnitType);
  }

  [Fact(DisplayName = "It should throw StoreNotFoundException when the store could not be found.")]
  public async Task It_should_throw_StoreNotFoundException_when_the_store_could_not_be_found()
  {
    ImportReceiptPayload payload = new()
    {
      StoreId = Guid.NewGuid()
    };
    ImportReceiptCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<StoreNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(new AggregateId(payload.StoreId), exception.AggregateId);
    Assert.Equal(nameof(payload.StoreId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ImportReceiptPayload payload = new()
    {
      Locale = "test"
    };
    ImportReceiptCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("LocaleValidator", error.ErrorCode);
    Assert.Equal("Locale", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the receipt lines are not valid.")]
  public async Task It_should_throw_ValidationException_when_the_receipt_lines_are_not_valid()
  {
    string[] lines =
    [
      "*27",
      "*27 FRUITS ET LEGUMES-",
      "06038385904\tPC POULET BBQ\tFPMRJ\t9.99\t9.99",
      "-6038385904\t\tFPMRJFPMRJFPMRJ\t9.99,",
      "ce28722d-ef39-4085-bc6a-3cff3672c69e\tPOIVRONS VERTS\tMRJ\t0.225 kg\t  -7.69  \t  1,73  "
    ];

    ImportReceiptPayload payload = new()
    {
      StoreId = _store.Id.ToGuid(),
      Lines = string.Join(Environment.NewLine, lines)
    };
    ImportReceiptCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal(11, exception.Errors.Count());

    Assert.Contains(exception.Errors, e => e.ErrorCode == "InvalidDepartmentLineColumnCount" && e.PropertyName == "Lines[0]" && (string?)e.AttemptedValue == lines[0]
      && e.ErrorMessage == "The department line does not have a valid column count (Expected=2, Actual=1).");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Lines[1].DepartmentNumber" && (string?)e.AttemptedValue == "27 FRUITS ET LEGUMES");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Lines[1].DepartmentName" && (string?)e.AttemptedValue == "");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InvalidItemLineColumnCount" && e.PropertyName == "Lines[2]" && (string?)e.AttemptedValue == lines[2]
      && e.ErrorMessage == "The item line does not have a valid column count (Expected=4 or 6, Actual=5).");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "Lines[3].Gtin" && (string?)e.AttemptedValue == "-6038385904");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Lines[3].Label" && (string?)e.AttemptedValue == "");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Lines[3].Flags" && (string?)e.AttemptedValue == "FPMRJFPMRJFPMRJ");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InvalidPrice" && e.PropertyName == "Lines[3].Price" && (string?)e.AttemptedValue == "9.99,"
      && e.ErrorMessage == "The specified value is not a valid price.");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Lines[4].Sku" && (string?)e.AttemptedValue == "ce28722d-ef39-4085-bc6a-3cff3672c69e");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InvalidQuantity" && e.PropertyName == "Lines[4].Quantity" && (string?)e.AttemptedValue == "0.225 kg"
      && e.ErrorMessage == "The specified value is not a valid quantity.");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "InvalidPrice" && e.PropertyName == "Lines[4].UnitPrice" && (string?)e.AttemptedValue == "-7.69"
      && e.ErrorMessage == "The specified value is not a valid price.");
  }
}
