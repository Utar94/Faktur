using Logitar.EventSourcing;

namespace Faktur.Domain.Receipts;

public record ReceiptId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ReceiptId(Guid id) : this(new AggregateId(id))
  {
  }

  public ReceiptId(AggregateId id)
  {
    AggregateId = id;
  }

  private ReceiptId(string value) : this(new AggregateId(value))
  {
  }

  public static ReceiptId NewId() => new(AggregateId.NewId());
  public static ReceiptId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new ReceiptId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
