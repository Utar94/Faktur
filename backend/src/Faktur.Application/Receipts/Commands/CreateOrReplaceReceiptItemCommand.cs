using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record CreateOrReplaceReceiptItemCommand(Guid ReceiptId, ushort ItemNumber, CreateOrReplaceReceiptItemPayload Payload, long? Version)
  : Activity, IRequest<CreateOrReplaceReceiptItemResult?>;
