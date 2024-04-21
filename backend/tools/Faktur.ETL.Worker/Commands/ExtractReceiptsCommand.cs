using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractReceiptsCommand(Mapper Mapper) : IRequest<IEnumerable<Receipt>>;
