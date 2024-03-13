using Faktur.Application.Activities;
using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.Application.Stores.Commands;

public record DeleteStoreCommand(Guid Id) : Activity, IRequest<Store?>;
