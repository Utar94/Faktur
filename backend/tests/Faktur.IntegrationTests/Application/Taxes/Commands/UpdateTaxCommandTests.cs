using Faktur.Contracts;
using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Taxes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateTaxCommandTests : IntegrationTests
{
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;

  public UpdateTaxCommandTests() : base()
  {
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync(_gst);
  }

  [Fact(DisplayName = "It should return null when the tax configuration cannot be found.")]
  public async Task It_should_return_null_when_the_tax_configuration_cannot_be_found()
  {
    UpdateTaxPayload payload = new();
    UpdateTaxCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw TaxCodeAlreadyUsedException when the tax code is already used.")]
  public async Task It_should_throw_TaxCodeAlreadyUsedException_when_the_tax_code_is_already_used()
  {
    TaxAggregate qst = new(new TaxCodeUnit("QST"), rate: 0.09975, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    qst.Update(ActorId);
    await _taxRepository.SaveAsync(qst);

    UpdateTaxPayload payload = new()
    {
      Code = qst.Code.Value
    };
    UpdateTaxCommand command = new(_gst.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<TaxCodeAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Code, exception.TaxCode);
    Assert.Equal(nameof(payload.Code), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateTaxPayload payload = new()
    {
      Rate = -0.05
    };
    UpdateTaxCommand command = new(_gst.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("Rate.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing tax configuration.")]
  public async Task It_should_update_an_existing_tax_configuration()
  {
    UpdateTaxPayload payload = new()
    {
      Flags = new Modification<string>("F")
    };
    UpdateTaxCommand command = new(_gst.Id.ToGuid(), payload);
    Tax? tax = await Mediator.Send(command);
    Assert.NotNull(tax);

    Assert.Equal(_gst.Id.ToGuid(), tax.Id);
    Assert.Equal(_gst.Version + 1, tax.Version);
    Assert.Equal(Actor, tax.CreatedBy);
    Assert.Equal(Actor, tax.UpdatedBy);
    Assert.True(tax.CreatedOn < tax.UpdatedOn);

    Assert.Equal(payload.Flags.Value, tax.Flags);
  }
}
