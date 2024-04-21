using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportReceiptsCommand(IEnumerable<Receipt> Receipts) : IRequest<int>;
