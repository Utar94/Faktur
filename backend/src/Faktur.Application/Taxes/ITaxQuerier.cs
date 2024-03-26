using Faktur.Contracts.Taxes;
using Faktur.Domain.Taxes;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Taxes;

public interface ITaxQuerier
{
  Task<Tax> ReadAsync(TaxAggregate tax, CancellationToken cancellationToken = default);
  Task<Tax?> ReadAsync(TaxId id, CancellationToken cancellationToken = default);
  Task<Tax?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Tax?> ReadAsync(string code, CancellationToken cancellationToken = default);
  Task<SearchResults<Tax>> SearchAsync(SearchTaxesPayload payload, CancellationToken cancellationToken = default);
}
