using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
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
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _qst;

  private readonly StoreAggregate _store;

  public ImportReceiptCommandTests() : base()
  {
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
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync([_gst, _qst]);
    await _storeRepository.SaveAsync(_store);
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
    Assert.Equal(1, receipt.Version);
    Assert.Equal(Actor, receipt.CreatedBy);
    Assert.Equal(Actor, receipt.UpdatedBy);
    Assert.Equal(receipt.CreatedOn, receipt.UpdatedOn);

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
    lines.AppendLine(string.Join('\t', "06041004754", "LAY'S CHIPS", "FPMRJ", "3.75")); // TODO(fpion): nothing exists
    lines.AppendLine("*27-FRUITS ET LEGUMES");
    lines.AppendLine(string.Join('\t', "bananas", "BANANES", "MRJ", "1.150", "1.52", "1.75")); // TODO(fpion): article & product exist
    lines.AppendLine(string.Join('\t', "4029", "ANANAS", "MRJ", "1.67")); // TODO(fpion): only article exists

    ImportReceiptPayload payload = new()
    {
      StoreId = _store.Id.ToGuid(),
      IssuedOn = DateTime.Now.AddDays(-1),
      Number = "103599",
      Locale = "fr",
      Lines = lines.ToString()
    };
    ImportReceiptCommand command = new(payload);
    Receipt receipt = await Mediator.Send(command);

    Assert.NotEqual(Guid.Empty, receipt.Id);
    Assert.Equal(1, receipt.Version);
    Assert.Equal(Actor, receipt.CreatedBy);
    Assert.Equal(Actor, receipt.UpdatedBy);
    Assert.Equal(receipt.CreatedOn, receipt.UpdatedOn);

    Assert.Equal(payload.IssuedOn?.ToUniversalTime(), receipt.IssuedOn);
    Assert.Equal(payload.Number, receipt.Number);

    Assert.Equal(3, receipt.ItemCount);
    Assert.Contains(receipt.Items, i => i.Number == 0 && i.Gtin == "06041004754" && i.Sku == null && i.Label == "LAY'S CHIPS" && i.Flags == "FPMRJ"
      && i.Quantity == 1.0d && i.UnitPrice == 3.75m && i.Price == 3.75m && i.Department == null && i.Category == null
      && i.CreatedBy.Equals(Actor) && i.UpdatedBy.Equals(Actor) && i.CreatedOn == i.UpdatedOn);
    DepartmentSummary department = new("27", "FRUITS ET LEGUMES");
    Assert.Contains(receipt.Items, i => i.Number == 1 && i.Gtin == null && i.Sku == "bananas" && i.Label == "BANANES" && i.Flags == "MRJ"
      && i.Quantity == 1.150d && i.UnitPrice == 1.52m && i.Price == 1.75m && i.Department == department && i.Category == null
      && i.CreatedBy.Equals(Actor) && i.UpdatedBy.Equals(Actor) && i.CreatedOn == i.UpdatedOn);
    Assert.Contains(receipt.Items, i => i.Number == 2 && i.Gtin == "4029" && i.Sku == null && i.Label == "ANANAS" && i.Flags == "MRJ"
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
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(receipt.Id).Value);
    Assert.NotNull(entity);
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
}
