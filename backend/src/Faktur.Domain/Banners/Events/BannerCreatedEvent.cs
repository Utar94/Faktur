using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Domain.Banners.Events;

public record BannerCreatedEvent(DisplayNameUnit DisplayName) : DomainEvent, INotification;
