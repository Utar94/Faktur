using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Application.Departments;

public interface IDepartmentQuerier
{
  Task<Department?> ReadAsync(string storeId, string number, CancellationToken cancellationToken = default);
  Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken = default);
}
