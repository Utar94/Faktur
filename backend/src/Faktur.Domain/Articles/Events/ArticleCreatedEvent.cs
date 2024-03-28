using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Domain.Articles.Events;

public record ArticleCreatedEvent(DisplayNameUnit DisplayName) : DomainEvent, INotification;
