using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Articles;

public record ArticleId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ArticleId(GtinUnit gtin) : this($"{gtin.Value}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}")
  {
  }

  public ArticleId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private ArticleId(string value) : this(new AggregateId(value))
  {
  }

  public static ArticleId Parse(string value, string propertyName)
  {
    new AggregateIdValidator(propertyName).ValidateAndThrow(value);

    return new ArticleId(value);
  }
}
