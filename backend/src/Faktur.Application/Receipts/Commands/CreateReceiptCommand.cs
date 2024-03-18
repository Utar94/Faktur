using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record CreateReceiptCommand(Guid StoreId, CreateReceiptPayload Payload) : Activity, IRequest<Receipt>;
