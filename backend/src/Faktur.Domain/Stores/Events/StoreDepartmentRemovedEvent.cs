using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDepartmentRemovedEvent(NumberUnit Number) : DomainEvent, INotification;
