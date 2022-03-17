using Faktur.Core.Models;

namespace Faktur.Core.Stores.Models
{
  public class DepartmentModel : AggregateModel
  {
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public string? Number { get; set; }
    public int StoreId { get; set; }
  }
}
