using Logitar.EventSourcing;

namespace Faktur.Domain.Banners;

public record BannerId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public BannerId(AggregateId id)
  {
    AggregateId = id;
  }

  private BannerId(string value) : this(new AggregateId(value))
  {
  }

  public static BannerId NewId() => new(AggregateId.NewId());
  public static BannerId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new BannerId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
