using Logitar.Faktur.Contracts.Banners;

namespace Logitar.Faktur.Contracts.Stores;

public class Store : Aggregate
{
  public string? Number { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public Address? Address { get; set; }
  public Phone? Phone { get; set; }

  public Banner? Banner { get; set; }
}
