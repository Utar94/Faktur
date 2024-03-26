using Faktur.Application.Taxes.Validators;
using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal class CreateTaxCommandHandler : IRequestHandler<CreateTaxCommand, Tax>
{
  private readonly IPublisher _publisher;
  private readonly ITaxQuerier _taxQuerier;

  public CreateTaxCommandHandler(IPublisher publisher, ITaxQuerier taxQuerier)
  {
    _publisher = publisher;
    _taxQuerier = taxQuerier;
  }

  public async Task<Tax> Handle(CreateTaxCommand command, CancellationToken cancellationToken)
  {
    CreateTaxPayload payload = command.Payload;
    new CreateTaxValidator().ValidateAndThrow(payload);

    TaxCodeUnit code = new(payload.Code);
    TaxAggregate tax = new(code, payload.Rate, command.ActorId)
    {
      Flags = FlagsUnit.TryCreate(payload.Flags)
    };

    tax.Update(command.ActorId);

    await _publisher.Publish(new SaveTaxCommand(tax), cancellationToken);

    return await _taxQuerier.ReadAsync(tax, cancellationToken);
  }
}
