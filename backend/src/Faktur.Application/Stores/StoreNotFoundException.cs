using Faktur.Domain.Stores;
using Logitar.EventSourcing;

namespace Faktur.Application.Stores;

public class StoreNotFoundException : AggregateNotFoundException<StoreAggregate>
{
  protected override string ErrorMessage { get; } = "The specified store could not be found.";

  public StoreNotFoundException(Guid storeId, string? propertyName = null) : base(new AggregateId(storeId), propertyName)
  {
  }
}
