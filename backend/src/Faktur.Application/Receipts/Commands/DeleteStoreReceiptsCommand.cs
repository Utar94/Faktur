using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal record DeleteStoreReceiptsCommand(ActorId ActorId, StoreAggregate Store) : INotification;
