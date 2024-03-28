using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record CategorizeReceiptCommand(Guid Id, CategorizeReceiptPayload Payload) : Activity, IRequest<Receipt?>;
