using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record ReplaceReceiptCommand(Guid Id, ReplaceReceiptPayload Payload, long? Version) : Activity, IRequest<Receipt?>;
