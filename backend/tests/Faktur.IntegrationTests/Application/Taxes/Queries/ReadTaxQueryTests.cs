using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Logitar.Portal.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Taxes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadTaxQueryTests : IntegrationTests
{
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;

  public ReadTaxQueryTests() : base()
  {
    _taxRepository = ServiceProvider.GetRequiredService<ITaxRepository>();

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    _gst.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _taxRepository.SaveAsync(_gst);
  }

  [Fact(DisplayName = "It should return null when the tax configuration cannot be found.")]
  public async Task It_should_return_null_when_the_tax_configuration_cannot_be_found()
  {
    ReadTaxQuery query = new(Guid.NewGuid(), Code: "QST");
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the tax configuration when it is found by code.")]
  public async Task It_should_return_the_tax_configuration_when_it_is_found_by_code()
  {
    ReadTaxQuery query = new(Id: null, _gst.Code.Value);
    Tax? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(_gst.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should return the tax configuration when it is found by ID.")]
  public async Task It_should_return_the_tax_configuration_when_it_is_found_by_Id()
  {
    ReadTaxQuery query = new(_gst.Id.ToGuid(), Code: null);
    Tax? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(_gst.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple tax configurations are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_tax_configurations_are_found()
  {
    TaxAggregate qst = new(new TaxCodeUnit("QST"), rate: 0.09975, ActorId)
    {
      Flags = new FlagsUnit("P")
    };
    qst.Update(ActorId);
    await _taxRepository.SaveAsync(qst);

    ReadTaxQuery query = new(_gst.Id.ToGuid(), qst.Code.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Tax>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
