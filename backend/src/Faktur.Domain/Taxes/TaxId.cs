using Logitar.EventSourcing;

namespace Faktur.Domain.Taxes;

public record TaxId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public TaxId(Guid id) : this(new AggregateId(id))
  {
  }

  public TaxId(AggregateId id)
  {
    AggregateId = id;
  }

  private TaxId(string value) : this(new AggregateId(value))
  {
  }

  public static TaxId NewId() => new(AggregateId.NewId());
  public static TaxId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new TaxId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
