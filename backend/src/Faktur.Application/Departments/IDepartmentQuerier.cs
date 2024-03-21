using Faktur.Contracts.Departments;
using Faktur.Domain.Stores;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Departments;

public interface IDepartmentQuerier
{
  Task<Department> ReadAsync(StoreAggregate store, NumberUnit number, CancellationToken cancellationToken = default);
  Task<Department?> ReadAsync(StoreId storeId, string number, CancellationToken cancellationToken = default);
  Task<Department?> ReadAsync(Guid storeId, string number, CancellationToken cancellationToken = default);
  Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken = default);
}
