using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractStoresCommand(Mapper Mapper) : IRequest<IEnumerable<Store>>;
