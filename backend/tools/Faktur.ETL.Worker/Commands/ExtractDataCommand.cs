using Logitar.Portal.Contracts.Actors;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractDataCommand(Actor Actor) : IRequest<ExtractedData>;
