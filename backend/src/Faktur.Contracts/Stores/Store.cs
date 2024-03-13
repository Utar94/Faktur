using Faktur.Contracts.Banners;
using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Stores;

public class Store : Aggregate
{
  public Banner? Banner { get; set; }
  public string? Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public Store() : this(string.Empty)
  {
  }

  public Store(string displayName)
  {
    DisplayName = displayName;
  }
}
