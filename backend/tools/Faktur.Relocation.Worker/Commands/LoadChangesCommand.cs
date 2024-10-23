using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using MediatR;

namespace Faktur.Relocation.Worker.Commands;

internal record LoadChangesCommand(IEnumerable<DomainEvent> Changes) : IRequest;

internal class LoadChangesCommandHandler : IRequestHandler<LoadChangesCommand>
{
  private readonly IEventBus _bus;
  private readonly ILogger<LoadChangesCommandHandler> _logger;

  public LoadChangesCommandHandler(IEventBus bus, ILogger<LoadChangesCommandHandler> logger)
  {
    _bus = bus;
    _logger = logger;
  }

  public async Task Handle(LoadChangesCommand command, CancellationToken cancellationToken)
  {
    // TODO(fpion): upsert events

    int count = command.Changes.Count();
    int index = 0;
    double percentage;
    foreach (DomainEvent change in command.Changes)
    {
      await _bus.PublishAsync(change, cancellationToken);

      index++;
      percentage = index / (double)count;
      _logger.LogInformation(
        "[{Index}/{Count}] ({Percentage}) Handled '{EventType}'.",
        index,
        count,
        percentage.ToString("P2"),
        change.GetType());
    }
  }
}
