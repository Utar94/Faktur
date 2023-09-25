using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Commands;

internal record CreateStoreCommand(CreateStorePayload Payload) : IRequest<AcceptedCommand>;
