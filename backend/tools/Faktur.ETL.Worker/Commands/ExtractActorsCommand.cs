using Logitar.Portal.Contracts.Actors;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractActorsCommand : IRequest<IEnumerable<Actor>>;
