using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Taxes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateTaxCommandTests : IntegrationTests
{
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;

  public CreateTaxCommandTests() : base()
  {
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05, ActorId)
    {
      Flags = new FlagsUnit("F")
    };
    _gst.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync(_gst);
  }

  [Fact(DisplayName = "It should create a new tax configuration.")]
  public async Task It_should_create_a_new_tax_configuration()
  {
    CreateTaxPayload payload = new("QST ", rate: 0.09975)
    {
      Flags = "P "
    };
    CreateTaxCommand command = new(payload);
    Tax tax = await Mediator.Send(command);

    Assert.NotEqual(Guid.Empty, tax.Id);
    Assert.Equal(2, tax.Version);
    Assert.Equal(Actor, tax.CreatedBy);
    Assert.Equal(Actor, tax.UpdatedBy);
    Assert.True(tax.CreatedOn < tax.UpdatedOn);

    Assert.Equal(payload.Code.Trim(), tax.Code);
    Assert.Equal(payload.Rate, tax.Rate);
    Assert.Equal(payload.Flags.Trim(), tax.Flags);
  }

  [Fact(DisplayName = "It should throw TaxCodeAlreadyUsedException when the tax code is already used.")]
  public async Task It_should_throw_TaxCodeAlreadyUsedException_when_the_tax_code_is_already_used()
  {
    CreateTaxPayload payload = new(_gst.Code.Value, _gst.Rate);
    CreateTaxCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TaxCodeAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Code, exception.TaxCode);
    Assert.Equal(nameof(payload.Code), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateTaxPayload payload = new("GST", rate: -0.05);
    CreateTaxCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.Rate), error.PropertyName);
  }
}
