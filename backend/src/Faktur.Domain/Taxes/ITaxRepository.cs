namespace Faktur.Domain.Taxes;

public interface ITaxRepository
{
  Task<TaxAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TaxAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken = default);
  Task<IEnumerable<TaxAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<TaxAggregate?> LoadAsync(TaxCodeUnit code, CancellationToken cancellationToken = default);

  Task SaveAsync(TaxAggregate tax, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<TaxAggregate> taxes, CancellationToken cancellationToken = default);
}
