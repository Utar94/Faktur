using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Commands;

internal record ReplaceStoreCommand(string Id, ReplaceStorePayload Payload, long? Version) : IRequest<AcceptedCommand>;
