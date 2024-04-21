using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportDataCommandHandler : IRequestHandler<ImportDataCommand>
{
  private readonly ILogger<ImportDataCommandHandler> _logger;
  private readonly ISender _sender;

  public ImportDataCommandHandler(ILogger<ImportDataCommandHandler> logger, ISender sender)
  {
    _logger = logger;
    _sender = sender;
  }

  public async Task Handle(ImportDataCommand command, CancellationToken cancellationToken)
  {
    ExtractedData extractedData = command.ExtractedData;

    int articles = await _sender.Send(new ImportArticlesCommand(extractedData.Articles), cancellationToken);
    _logger.LogInformation("Saved {Count} articles.", articles);

    int banners = await _sender.Send(new ImportBannersCommand(extractedData.Banners), cancellationToken);
    _logger.LogInformation("Saved {Count} banners.", banners);

    int stores = await _sender.Send(new ImportStoresCommand(extractedData.Stores), cancellationToken);
    _logger.LogInformation("Saved {Count} stores.", stores);

    int products = await _sender.Send(new ImportProductsCommand(extractedData.Products), cancellationToken);
    _logger.LogInformation("Saved {Count} products.", products);

    // TODO(fpion): import receipts
  }
}
