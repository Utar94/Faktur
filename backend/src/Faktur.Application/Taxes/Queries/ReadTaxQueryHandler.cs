using Faktur.Contracts.Taxes;
using Logitar.Portal.Contracts;
using MediatR;

namespace Faktur.Application.Taxes.Queries;

internal class ReadTaxQueryHandler : IRequestHandler<ReadTaxQuery, Tax?>
{
  private readonly ITaxQuerier _taxQuerier;

  public ReadTaxQueryHandler(ITaxQuerier taxQuerier)
  {
    _taxQuerier = taxQuerier;
  }

  public async Task<Tax?> Handle(ReadTaxQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Tax> taxes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Tax? tax = await _taxQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (tax != null)
      {
        taxes[tax.Id] = tax;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Code))
    {
      Tax? tax = await _taxQuerier.ReadAsync(query.Code, cancellationToken);
      if (tax != null)
      {
        taxes[tax.Id] = tax;
      }
    }

    if (taxes.Count > 1)
    {
      throw new TooManyResultsException<Tax>(expectedCount: 1, actualCount: taxes.Count);
    }

    return taxes.Values.SingleOrDefault();
  }
}
