using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

public record ReadReceiptItemQuery(Guid ReceiptId, ushort ItemNumber) : IRequest<ReceiptItem?>;
