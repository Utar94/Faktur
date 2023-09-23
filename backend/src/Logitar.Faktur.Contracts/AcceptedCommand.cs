using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Contracts;

public record AcceptedCommand
{
  public string AggregateId { get; set; } = string.Empty;
  public long AggregateVersion { get; set; }

  public Actor Actor { get; set; } = new();
  public DateTime Timestamp { get; set; }
}
