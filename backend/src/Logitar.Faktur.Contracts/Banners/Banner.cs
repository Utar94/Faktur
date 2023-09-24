namespace Logitar.Faktur.Contracts.Banners;

public class Banner : Aggregate
{
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
