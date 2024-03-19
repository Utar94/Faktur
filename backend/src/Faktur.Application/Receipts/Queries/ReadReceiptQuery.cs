using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

public record ReadReceiptQuery(Guid Id) : IRequest<Receipt?>;
