using Faktur.Contracts.Departments;

namespace Faktur.Models.Departments;

public record SearchDepartmentsModel : SearchModel
{
  public SearchDepartmentsPayload ToPayload(Guid storeId)
  {
    SearchDepartmentsPayload payload = new(storeId);
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new DepartmentSortOption(Enum.Parse<DepartmentSort>(sort)));
      }
      else
      {
        DepartmentSort field = Enum.Parse<DepartmentSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new DepartmentSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
