using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Banners;

public record BannerId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  private BannerId(string value) : this(new AggregateId(value))
  {
  }
  public BannerId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static BannerId Parse(string value, string propertyName)
  {
    new AggregateIdValidator(propertyName).ValidateAndThrow(value);

    return new BannerId(value);
  }
}
