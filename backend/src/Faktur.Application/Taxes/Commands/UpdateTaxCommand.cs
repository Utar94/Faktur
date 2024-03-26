using Faktur.Application.Activities;
using Faktur.Contracts.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

public record UpdateTaxCommand(Guid Id, UpdateTaxPayload Payload) : Activity, IRequest<Tax?>;
