namespace Faktur.Domain.Taxes;

public interface ITaxRepository
{
  Task SaveAsync(TaxAggregate tax, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TaxAggregate> taxes, CancellationToken cancellationToken = default);
}
