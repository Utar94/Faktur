using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Departments;

public record SearchDepartmentsPayload : SearchPayload
{
  public string StoreId { get; set; } = string.Empty;

  public new List<DepartmentSortOption> Sort { get; set; } = new();
}
