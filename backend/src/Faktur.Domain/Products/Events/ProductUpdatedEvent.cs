using Faktur.Contracts;
using Faktur.Contracts.Products;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Products.Events;

public record ProductUpdatedEvent : DomainEvent, INotification
{
  public Modification<NumberUnit>? DepartmentNumber { get; set; }

  public Modification<SkuUnit>? Sku { get; set; }
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public Modification<FlagsUnit>? Flags { get; set; }

  public Modification<decimal?>? UnitPrice { get; set; }
  public Modification<UnitType?>? UnitType { get; set; }

  public bool HasChanges => DepartmentNumber != null
    || Sku != null || DisplayName != null || Description != null
    || Flags != null
    || UnitPrice != null || UnitType != null;
}
