using Faktur.Application.Activities;
using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

public record DeleteReceiptCommand(Guid Id) : Activity, IRequest<Receipt>;
