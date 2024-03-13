using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Departments;

public interface IDepartmentQuerier
{
  Task<Department> ReadAsync(StoreAggregate store, NumberUnit number, CancellationToken cancellationToken = default);
  Task<Department?> ReadAsync(StoreId id, string number, CancellationToken cancellationToken = default);
  Task<Department?> ReadAsync(Guid id, string number, CancellationToken cancellationToken = default);
  Task<SearchResults<Department>> SearchAsync(Guid id, SearchDepartmentsPayload payload, CancellationToken cancellationToken = default);
}
