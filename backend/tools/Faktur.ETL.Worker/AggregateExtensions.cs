using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Receipts.Events;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;

namespace Faktur.ETL.Worker;

internal static class AggregateExtensions
{
  public static void SetDates(this AggregateRoot aggregate, Aggregate model)
  {
    foreach (DomainEvent change in aggregate.Changes)
    {
      if (change.Version > 1)
      {
        change.OccurredOn = model.UpdatedOn;
      }
      else
      {
        change.OccurredOn = model.CreatedOn;
      }
    }
  }

  public static void SetDates(this ReceiptAggregate aggregate, Receipt model)
  {
    foreach (DomainEvent change in aggregate.Changes)
    {
      if (change is ReceiptCategorizedEvent categorized && model.ProcessedOn.HasValue)
      {
        categorized.OccurredOn = model.ProcessedOn.Value == default ? model.UpdatedOn : model.ProcessedOn.Value;
      }
      else if (change.Version > 1)
      {
        change.OccurredOn = model.UpdatedOn;
      }
      else
      {
        change.OccurredOn = model.CreatedOn;
      }
    }
  }
}
