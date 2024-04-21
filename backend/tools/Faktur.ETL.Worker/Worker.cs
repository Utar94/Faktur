namespace Faktur.ETL.Worker;

public class Worker : BackgroundService
{
  protected override Task ExecuteAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
