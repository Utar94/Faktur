using Logitar.EventSourcing;

namespace Faktur.Domain.Articles;

public record ArticleId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ArticleId(Guid id) : this(new AggregateId(id))
  {
  }

  public ArticleId(AggregateId id)
  {
    AggregateId = id;
  }

  private ArticleId(string value) : this(new AggregateId(value))
  {
  }

  public static ArticleId NewId() => new(AggregateId.NewId());
  public static ArticleId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new ArticleId(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}
