using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Departments;

public record SearchDepartmentsPayload : SearchPayload
{
  public Guid StoreId { get; set; }

  public new List<DepartmentSortOption> Sort { get; set; } = [];

  public SearchDepartmentsPayload() : this(Guid.Empty)
  {
  }

  public SearchDepartmentsPayload(Guid storeId)
  {
    StoreId = storeId;
  }
}
