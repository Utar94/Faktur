using Faktur.Contracts.Banners;
using Faktur.Contracts.Departments;
using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Stores;

public class Store : Aggregate
{
  public string? Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public int DepartmentCount { get; set; }
  public List<Department> Departments { get; set; }

  public Banner? Banner { get; set; }

  public Store() : this(string.Empty)
  {
  }

  public Store(string displayName)
  {
    DisplayName = displayName;
    Departments = [];
  }
}
