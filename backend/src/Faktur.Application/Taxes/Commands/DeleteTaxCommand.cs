using Faktur.Application.Activities;
using Faktur.Contracts.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

public record DeleteTaxCommand(Guid Id) : Activity, IRequest<Tax?>;
