using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker.Commands;

internal record ExtractChangesCommand : IRequest<IReadOnlyCollection<DomainEvent>>;

internal class ExtractChangesCommandHandler : IRequestHandler<ExtractChangesCommand, IReadOnlyCollection<DomainEvent>>
{
  private readonly IEventSerializer _eventSerializer;
  private readonly ILogger<ExtractChangesCommandHandler> _logger;
  private readonly SourceContext _source;

  public ExtractChangesCommandHandler(IEventSerializer eventSerializer, ILogger<ExtractChangesCommandHandler> logger, SourceContext source)
  {
    _eventSerializer = eventSerializer;
    _logger = logger;
    _source = source;
  }

  public async Task<IReadOnlyCollection<DomainEvent>> Handle(ExtractChangesCommand _, CancellationToken cancellationToken)
  {
    EventEntity[] events = await _source.Events.AsNoTracking().ToArrayAsync(cancellationToken);
    _logger.LogInformation("Extracted {EventCount} {EventText} from source database.", events.Length, events.Length > 1 ? "events" : "event");

    return events.Select(_eventSerializer.Deserialize).ToArray().AsReadOnly();
  }
}
