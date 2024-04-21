using Faktur.Contracts.Articles;
using Faktur.Contracts.Banners;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Contracts.Stores;
using Logitar.Portal.Contracts.Actors;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractDataCommandHandler : IRequestHandler<ExtractDataCommand, ExtractedData>
{
  private readonly ILogger<ExtractDataCommandHandler> _logger;
  private readonly ISender _sender;

  public ExtractDataCommandHandler(ILogger<ExtractDataCommandHandler> logger, ISender sender)
  {
    _logger = logger;
    _sender = sender;
  }

  public async Task<ExtractedData> Handle(ExtractDataCommand _, CancellationToken cancellationToken)
  {
    IEnumerable<Actor> actors = await _sender.Send(new ExtractActorsCommand(), cancellationToken);
    Mapper mapper = new(actors);

    IEnumerable<Article> articles = await _sender.Send(new ExtractArticlesCommand(mapper), cancellationToken);
    _logger.LogInformation("Extracted {Count} articles.", articles.Count());

    IEnumerable<Banner> banners = await _sender.Send(new ExtractBannersCommand(mapper), cancellationToken);
    _logger.LogInformation("Extracted {Count} banners.", banners.Count());

    IEnumerable<Store> stores = await _sender.Send(new ExtractStoresCommand(mapper), cancellationToken);
    _logger.LogInformation("Extracted {Count} stores.", stores.Count());

    IEnumerable<Product> products = await _sender.Send(new ExtractProductsCommand(mapper), cancellationToken);
    _logger.LogInformation("Extracted {Count} products.", products.Count());

    IEnumerable<Receipt> receipts = await _sender.Send(new ExtractReceiptsCommand(mapper), cancellationToken);
    _logger.LogInformation("Extracted {Count} receipts.", receipts.Count());

    return new ExtractedData(articles, banners, stores, products, receipts);
  }
}
