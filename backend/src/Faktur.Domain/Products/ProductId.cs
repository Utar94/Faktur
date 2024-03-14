using Logitar.EventSourcing;

namespace Faktur.Domain.Products;

public record ProductId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ProductId(Guid id) : this(new AggregateId(id))
  {
  }

  public ProductId(AggregateId id)
  {
    AggregateId = id;
  }

  private ProductId(string value) : this(new AggregateId(value))
  {
  }

  public static ProductId NewId() => new(AggregateId.NewId());
  public static ProductId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new ProductId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
