using Faktur.Relocation.Worker.Commands;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Relocation.Worker;

internal class Worker : BackgroundService
{
  private const string ActorIdKey = "ActorId";
  private const string GenericErrorMessage = "An unhandled exception occurred.";

  private readonly IConfiguration _configuration;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly ILogger<Worker> _logger;
  private readonly IServiceProvider _serviceProvider;

  private LogLevel _result = LogLevel.Information; // NOTE(fpion): "Information" means success.

  public Worker(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger, IServiceProvider serviceProvider)
  {
    _configuration = configuration;
    _hostApplicationLifetime = hostApplicationLifetime;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    using IServiceScope scope = _serviceProvider.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    try
    {
      IReadOnlyCollection<DomainEvent> changes = await mediator.Send(new ExtractChangesCommand(), cancellationToken);

      ActorId actorId = GetActorId();
      foreach (DomainEvent change in changes)
      {
        change.ActorId = actorId;
      }

      await mediator.Send(new LoadChangesCommand(changes), cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, GenericErrorMessage);
      _result = LogLevel.Error;

      Environment.ExitCode = exception.HResult;
    }
    finally
    {
      chrono.Stop();

      long seconds = chrono.ElapsedMilliseconds / 1000;
      string secondText = seconds <= 1 ? "second" : "seconds";
      switch (_result)
      {
        case LogLevel.Error:
          _logger.LogError("Worker failed after {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        case LogLevel.Warning:
          _logger.LogWarning("Worker completed with warnings in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        default:
          _logger.LogInformation("Worker succeeded in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
      }

      _hostApplicationLifetime.StopApplication();
    }
  }
  private ActorId GetActorId()
  {
    string? value = _configuration.GetValue<string>(ActorIdKey);
    return string.IsNullOrWhiteSpace(value)
      ? throw new InvalidOperationException($"The configuration '{ActorIdKey}' is required.")
      : new(value);
  }
}
