using Faktur.Domain.Taxes;
using Faktur.Domain.Taxes.Events;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal class SaveTaxCommandHandler : INotificationHandler<SaveTaxCommand>
{
  private readonly ITaxRepository _taxRepository;

  public SaveTaxCommandHandler(ITaxRepository taxRepository)
  {
    _taxRepository = taxRepository;
  }

  public async Task Handle(SaveTaxCommand command, CancellationToken cancellationToken)
  {
    TaxAggregate tax = command.Tax;

    bool hasCodeChanged = false;
    foreach (DomainEvent change in tax.Changes)
    {
      if (change is TaxCreatedEvent || change is TaxUpdatedEvent updated && updated.Code != null)
      {
        hasCodeChanged = true;
      }
    }

    if (hasCodeChanged)
    {
      TaxAggregate? other = await _taxRepository.LoadAsync(tax.Code, cancellationToken);
      if (other != null && !other.Equals(tax))
      {
        throw new TaxCodeAlreadyUsedException(tax.Code, nameof(tax.Code));
      }
    }

    await _taxRepository.SaveAsync(tax, cancellationToken);
  }
}
