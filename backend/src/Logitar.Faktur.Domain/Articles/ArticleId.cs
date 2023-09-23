using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Articles;

public record ArticleId
{
  public AggregateId AggregateId { get; }

  private ArticleId(string value) : this(new AggregateId(value))
  {
  }
  public ArticleId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static ArticleId NewId(GtinUnit? gtin = null)
  {
    return gtin == null
      ? new(AggregateId.NewId())
      : new($"{gtin.Value}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
  }

  public static ArticleId Parse(string value, string propertyName)
  {
    new AggregateIdValidator(propertyName).ValidateAndThrow(value);

    return new ArticleId(value);
  }
}
