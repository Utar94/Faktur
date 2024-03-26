using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Taxes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteTaxCommandTests : IntegrationTests
{
  private readonly ITaxRepository _taxRepository;

  private readonly TaxAggregate _gst;

  public DeleteTaxCommandTests() : base()
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

  [Fact(DisplayName = "It should delete an existing tax configuration.")]
  public async Task It_should_delete_an_existing_tax_configuration()
  {
    DeleteTaxCommand command = new(_gst.Id.ToGuid());
    Tax? tax = await Mediator.Send(command);
    Assert.NotNull(tax);
    Assert.Equal(_gst.Id.ToGuid(), tax.Id);

    TaxEntity? entity = await FakturContext.Taxes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(tax.Id).Value);
    Assert.Null(entity);
  }

  [Fact(DisplayName = "It should return null when the tax configuration cannot be found.")]
  public async Task It_should_return_null_when_the_tax_configuration_cannot_be_found()
  {
    DeleteTaxCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
