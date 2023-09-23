using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Domain.Articles.Events;

public record ArticleUpdatedEvent : DomainEvent, INotification
{
  public Modification<string>? Gtin { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public bool HasChanges => Gtin != null || DisplayName != null || Description != null;
}
