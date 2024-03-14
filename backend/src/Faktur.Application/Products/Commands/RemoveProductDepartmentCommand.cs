using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal record RemoveProductDepartmentCommand(ActorId ActorId, StoreAggregate Store, NumberUnit DepartmentNumber) : INotification;
