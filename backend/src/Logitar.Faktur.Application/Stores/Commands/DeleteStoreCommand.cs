using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Commands;

internal record DeleteStoreCommand(string Id) : IRequest<AcceptedCommand>;
