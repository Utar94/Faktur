using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Departments;

public interface IDepartmentService
{
  Task<Department?> ReadAsync(string storeId, string number, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> RemoveAsync(string storeId, string number, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> SaveAsync(string storeId, string number, SaveDepartmentPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> UpdateAsync(string storeId, string number, UpdateDepartmentPayload payload, CancellationToken cancellationToken = default);
}
