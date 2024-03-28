using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDepartmentSavedEvent(NumberUnit Number, DepartmentUnit Department) : DomainEvent, INotification;
