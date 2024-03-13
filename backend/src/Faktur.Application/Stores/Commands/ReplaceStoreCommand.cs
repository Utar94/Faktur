using Faktur.Application.Activities;
using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.Application.Stores.Commands;

public record ReplaceStoreCommand(Guid Id, ReplaceStorePayload Payload, long? Version) : Activity, IRequest<Store?>;
