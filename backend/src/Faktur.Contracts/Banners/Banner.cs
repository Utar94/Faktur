using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Banners;

public class Banner : Aggregate
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  // TODO(fpion): Stores

  public Banner() : this(string.Empty)
  {
  }

  public Banner(string displayName)
  {
    DisplayName = displayName;
  }
}
