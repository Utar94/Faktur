using Faktur.ETL.Worker.Commands;
using MediatR;
using System.Diagnostics;

namespace Faktur.ETL.Worker;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;
  private readonly IServiceProvider _serviceProvider;

  public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();

    using IServiceScope scope = _serviceProvider.CreateScope();
    ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

    ExtractedData extractedData = await sender.Send(new ExtractDataCommand(), cancellationToken);
    await sender.Send(new ImportDataCommand(extractedData), cancellationToken);

    chrono.Stop();
    _logger.LogInformation("Operation completed in {Elapsed}ms.", chrono.ElapsedMilliseconds);
  }
}
