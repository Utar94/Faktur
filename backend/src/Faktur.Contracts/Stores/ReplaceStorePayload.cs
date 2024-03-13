namespace Faktur.Contracts.Stores;

public record ReplaceStorePayload
{
  public Guid? BannerId { get; set; }
  public string? Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public ReplaceStorePayload() : this(string.Empty)
  {
  }

  public ReplaceStorePayload(string displayName)
  {
    DisplayName = displayName;
  }
}
