using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Taxes.Events;

public record TaxCreatedEvent(TaxCodeUnit Code, double Rate) : DomainEvent, INotification;
