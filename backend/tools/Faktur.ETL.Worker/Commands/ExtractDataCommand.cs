using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractDataCommand : IRequest<ExtractedData>;
