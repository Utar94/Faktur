using Faktur.Contracts;
using Faktur.Domain.Products;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Taxes.Events;

public record TaxUpdatedEvent : DomainEvent, INotification
{
  public TaxCodeUnit? Code { get; set; }
  public double? Rate { get; set; }

  public Modification<FlagsUnit>? Flags { get; set; }

  public bool HasChanges => Code != null || Rate != null || Flags != null;
}
