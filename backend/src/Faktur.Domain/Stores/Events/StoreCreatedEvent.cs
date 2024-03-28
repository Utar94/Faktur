using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreCreatedEvent(DisplayNameUnit DisplayName) : DomainEvent, INotification;
