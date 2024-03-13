namespace Faktur.Contracts.Stores;

public record UpdateStorePayload
{
  public Modification<Guid?>? BannerId { get; set; }
  public Modification<string>? Number { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
