using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchDepartmentsQuery : SearchQuery
{
  public SearchDepartmentsPayload ToPayload(string storeId)
  {
    SearchDepartmentsPayload payload = new();

    ApplyQuery(payload);

    payload.StoreId = storeId;

    List<SortOption> sort = ((SearchPayload)payload).Sort;
    payload.Sort = new List<DepartmentSortOption>(sort.Capacity);
    foreach (SortOption option in sort)
    {
      if (Enum.TryParse(option.Field, out DepartmentSort field))
      {
        payload.Sort.Add(new DepartmentSortOption(field, option.IsDescending));
      }
    }

    return payload;
  }
}
