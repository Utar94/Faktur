using Faktur.Application.Activities;
using Faktur.Contracts.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

public record CreateTaxCommand(CreateTaxPayload Payload) : Activity, IRequest<Tax>;
