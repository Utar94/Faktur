using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreCreatedEvent(DisplayNameUnit DisplayName) : DomainEvent, INotification;
