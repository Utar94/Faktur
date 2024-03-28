using Faktur.Domain.Articles;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Products.Events;

public record ProductCreatedEvent(StoreId StoreId, ArticleId ArticleId) : DomainEvent, INotification;
