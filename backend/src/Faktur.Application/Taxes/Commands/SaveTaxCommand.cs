using Faktur.Domain.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

internal record SaveTaxCommand(TaxAggregate Tax) : INotification;
