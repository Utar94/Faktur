using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Stores;

public record StoreId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public StoreId(BannerAggregate banner, StoreNumberUnit number) : this($"{banner.Id.Value}-{number.Value}")
  {
  }

  public StoreId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private StoreId(string value) : this(new AggregateId(value))
  {
  }

  public static StoreId Parse(string value, string propertyName)
  {
    new AggregateIdValidator(propertyName).ValidateAndThrow(value);

    return new StoreId(value);
  }
}
