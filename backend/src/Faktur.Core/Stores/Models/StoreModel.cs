using Faktur.Core.Models;

namespace Faktur.Core.Stores.Models
{
  public class StoreModel : AggregateModel
  {
    public int? BannerId { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public string? Number { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }
  }
}
