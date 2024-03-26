using Faktur.Contracts.Taxes;
using Faktur.Domain.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal class DeleteTaxCommandHandler : IRequestHandler<DeleteTaxCommand, Tax?>
{
  private readonly ITaxQuerier _taxQuerier;
  private readonly ITaxRepository _taxRepository;

  public DeleteTaxCommandHandler(ITaxQuerier taxQuerier, ITaxRepository taxRepository)
  {
    _taxQuerier = taxQuerier;
    _taxRepository = taxRepository;
  }

  public async Task<Tax?> Handle(DeleteTaxCommand command, CancellationToken cancellationToken)
  {
    TaxAggregate? tax = await _taxRepository.LoadAsync(command.Id, cancellationToken);
    if (tax == null)
    {
      return null;
    }
    Tax result = await _taxQuerier.ReadAsync(tax, cancellationToken);

    tax.Delete(command.ActorId);

    await _taxRepository.SaveAsync(tax, cancellationToken);

    return result;
  }
}
