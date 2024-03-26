using Faktur.Application.Taxes.Validators;
using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal class ReplaceTaxCommandHandler : IRequestHandler<ReplaceTaxCommand, Tax?>
{
  private readonly ITaxQuerier _taxQuerier;
  private readonly ITaxRepository _taxRepository;

  public ReplaceTaxCommandHandler(ITaxQuerier taxQuerier, ITaxRepository taxRepository)
  {
    _taxQuerier = taxQuerier;
    _taxRepository = taxRepository;
  }

  public async Task<Tax?> Handle(ReplaceTaxCommand command, CancellationToken cancellationToken)
  {
    ReplaceTaxPayload payload = command.Payload;
    new ReplaceTaxValidator().ValidateAndThrow(payload);

    TaxAggregate? tax = await _taxRepository.LoadAsync(command.Id, cancellationToken);
    if (tax == null)
    {
      return null;
    }
    TaxAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _taxRepository.LoadAsync(command.Id, command.Version.Value, cancellationToken);
    }

    TaxCodeUnit code = new(payload.Code);
    if (reference == null || code != reference.Code)
    {
      tax.Code = code;
    }
    if (reference == null || payload.Rate != reference.Rate)
    {
      tax.Rate = payload.Rate;
    }

    FlagsUnit? flags = FlagsUnit.TryCreate(payload.Flags);
    if (reference == null || flags != reference.Flags)
    {
      tax.Flags = flags;
    }

    tax.Update(command.ActorId);

    await _taxRepository.SaveAsync(tax, cancellationToken);

    return await _taxQuerier.ReadAsync(tax, cancellationToken);
  }
}
