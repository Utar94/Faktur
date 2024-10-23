using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker.Commands;

internal record ExtractChangesCommand : IRequest<IReadOnlyCollection<DomainEvent>>;

internal class ExtractChangesCommandHandler : IRequestHandler<ExtractChangesCommand, IReadOnlyCollection<DomainEvent>>
{
  private const int BatchSize = 1000;

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
    int total = await _source.Events.AsNoTracking().CountAsync(cancellationToken);
    List<DomainEvent> changes = new(capacity: total);

    int batchCount = total / BatchSize;
    if (total % BatchSize > 0)
    {
      batchCount++;
    }

    double percentage;
    for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
    {
      int skip = batchIndex * BatchSize;
      EventEntity[] events = await _source.Events.AsNoTracking()
        .OrderBy(e => e.EventId)
        .Skip(skip)
        .Take(BatchSize)
        .ToArrayAsync(cancellationToken);
      changes.AddRange(events.Select(_eventSerializer.Deserialize));

      percentage = changes.Count / (double)total;
      _logger.LogInformation("[{Count}/{Total}] ({Percentage}) Extracted {EventCount} {EventText} from source database.",
        changes.Count,
        total,
        percentage.ToString("P2"),
        events.Length,
        events.Length > 1 ? "events" : "event");
    }

    return changes.AsReadOnly();
  }
}
