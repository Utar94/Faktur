using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Departments;

public record DepartmentSortOption : SortOption
{
  public new DepartmentSort Field { get; set; }

  public DepartmentSortOption() : this(DepartmentSort.UpdatedOn, isDescending: true)
  {
  }
  public DepartmentSortOption(DepartmentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }
}
