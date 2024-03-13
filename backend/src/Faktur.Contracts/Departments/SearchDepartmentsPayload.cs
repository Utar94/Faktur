using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Departments;

public record SearchDepartmentsPayload : SearchPayload
{
  public new List<DepartmentSortOption> Sort { get; set; } = [];
}
