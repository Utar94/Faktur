using Logitar.EventSourcing;

namespace Faktur.Domain.Stores;

public record StoreId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public StoreId(Guid id) : this(new AggregateId(id))
  {
  }

  public StoreId(AggregateId id)
  {
    AggregateId = id;
  }

  private StoreId(string value) : this(new AggregateId(value))
  {
  }

  public static StoreId NewId() => new(AggregateId.NewId());
  public static StoreId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new StoreId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
