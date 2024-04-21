using Logitar.Portal.Contracts.Actors;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportDataCommand(Actor Actor, ExtractedData ExtractedData) : IRequest;
