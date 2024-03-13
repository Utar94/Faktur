using Faktur.Application.Activities;
using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.Application.Stores.Commands;

public record UpdateStoreCommand(Guid Id, UpdateStorePayload Payload) : Activity, IRequest<Store?>;
