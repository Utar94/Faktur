using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Departments;

public interface IDepartmentQuerier
{
  Task<Department?> ReadAsync(StoreId storeId, DepartmentNumberUnit number, CancellationToken cancellationToken = default);
  Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken = default);
}
