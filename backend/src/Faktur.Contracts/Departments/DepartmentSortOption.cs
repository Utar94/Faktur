using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Departments;

public record DepartmentSortOption : SortOption
{
  public new DepartmentSort Field
  {
    get => Enum.Parse<DepartmentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public DepartmentSortOption() : this(DepartmentSort.UpdatedOn, isDescending: true)
  {
  }

  public DepartmentSortOption(DepartmentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
