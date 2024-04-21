using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportStoresCommand(IEnumerable<Store> Stores) : IRequest<int>;
