using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Articles.Events;

public record ArticleUpdatedEvent : DomainEvent, INotification
{
  public Modification<GtinUnit>? Gtin { get; set; }
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => Gtin != null || DisplayName != null || Description != null;
}
