using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportDataCommand(ExtractedData ExtractedData) : IRequest;
