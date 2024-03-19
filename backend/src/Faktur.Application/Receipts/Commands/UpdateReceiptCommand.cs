using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record UpdateReceiptCommand(Guid Id, UpdateReceiptPayload Payload) : Activity, IRequest<Receipt?>;
