namespace Faktur.Contracts.Stores;

public record CreateStorePayload
{
  public Guid? BannerId { get; set; }
  public string? Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateStorePayload() : this(string.Empty)
  {
  }

  public CreateStorePayload(string displayName)
  {
    DisplayName = displayName;
  }
}
