using Logitar.Portal.Contracts.Actors;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportTaxesCommand(Actor Actor) : IRequest<int>;
