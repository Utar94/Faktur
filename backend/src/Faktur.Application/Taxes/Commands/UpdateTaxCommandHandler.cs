using Faktur.Application.Taxes.Validators;
using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal class UpdateTaxCommandHandler : IRequestHandler<UpdateTaxCommand, Tax?>
{
  private readonly IPublisher _publisher;
  private readonly ITaxQuerier _taxQuerier;
  private readonly ITaxRepository _taxRepository;

  public UpdateTaxCommandHandler(IPublisher publisher, ITaxQuerier taxQuerier, ITaxRepository taxRepository)
  {
    _publisher = publisher;
    _taxQuerier = taxQuerier;
    _taxRepository = taxRepository;
  }

  public async Task<Tax?> Handle(UpdateTaxCommand command, CancellationToken cancellationToken)
  {
    UpdateTaxPayload payload = command.Payload;
    new UpdateTaxValidator().ValidateAndThrow(payload);

    TaxAggregate? tax = await _taxRepository.LoadAsync(command.Id, cancellationToken);
    if (tax == null)
    {
      return null;
    }

    TaxCodeUnit? code = TaxCodeUnit.TryCreate(payload.Code);
    if (code != null)
    {
      tax.Code = code;
    }
    if (payload.Rate.HasValue)
    {
      tax.Rate = payload.Rate.Value;
    }

    if (payload.Flags != null)
    {
      tax.Flags = FlagsUnit.TryCreate(payload.Flags.Value);
    }

    tax.Update(command.ActorId);

    await _publisher.Publish(new SaveTaxCommand(tax), cancellationToken);

    return await _taxQuerier.ReadAsync(tax, cancellationToken);
  }
}
