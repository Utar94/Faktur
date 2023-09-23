namespace Logitar.Faktur.Contracts.Actors;

public class Actor
{
  public string Id { get; set; } = "SYSTEM";
  public ActorType Type { get; set; } = ActorType.System;
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = ActorType.System.ToString();
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }
}
