using Faktur.Contracts.Stores;
using Logitar.Portal.Contracts.Actors;

namespace Faktur.Contracts.Departments;

public class Department
{
  public Store Store { get; set; }
  public string Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public Department() : this(new Store(), string.Empty, string.Empty)
  {
  }

  public Department(Store store, string number, string displayName)
  {
    Store = store;
    Number = number;
    DisplayName = displayName;
  }

  public override bool Equals(object? obj) => obj is Department department && department.Store.Id == Store.Id && department.Number == Number;
  public override int GetHashCode() => HashCode.Combine(GetType(), Store.Id, Number);
  public override string ToString() => $"{DisplayName} | ({GetType()} #{Number}, StoreId={Store.Id})";
}
