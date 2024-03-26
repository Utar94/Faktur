using Faktur.Contracts.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Queries;

public record ReadTaxQuery(Guid? Id, string? Code) : IRequest<Tax?>;
