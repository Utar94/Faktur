using Faktur.ETL.Worker.Commands;
using Logitar.Portal.Contracts.Actors;
using MediatR;
using System.Diagnostics;

namespace Faktur.ETL.Worker;

public class Worker : BackgroundService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<Worker> _logger;
  private readonly IServiceProvider _serviceProvider;

  public Worker(IConfiguration configuration, ILogger<Worker> logger, IServiceProvider serviceProvider)
  {
    _configuration = configuration;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();

    using IServiceScope scope = _serviceProvider.CreateScope();
    ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

    Actor actor = _configuration.GetSection("Actor").Get<Actor>() ?? Actor.System;

    ExtractedData extractedData = await sender.Send(new ExtractDataCommand(actor), cancellationToken);
    await sender.Send(new ImportDataCommand(actor, extractedData), cancellationToken);

    chrono.Stop();
    _logger.LogInformation("Operation completed in {Elapsed}ms.", chrono.ElapsedMilliseconds);
  }
}
