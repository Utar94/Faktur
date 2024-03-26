using Faktur.Application.Activities;
using Faktur.Contracts.Taxes;
using MediatR;

namespace Faktur.Application.Taxes.Commands;

public record ReplaceTaxCommand(Guid Id, ReplaceTaxPayload Payload, long? Version) : Activity, IRequest<Tax?>;
