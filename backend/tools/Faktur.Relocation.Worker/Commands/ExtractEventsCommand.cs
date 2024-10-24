using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker.Commands;

internal record ExtractEventsCommand : IRequest<IReadOnlyCollection<EventEntity>>;

internal class ExtractEventsCommandHandler : IRequestHandler<ExtractEventsCommand, IReadOnlyCollection<EventEntity>>
{
  private const int BatchSize = 1000;

  private readonly ILogger<ExtractEventsCommandHandler> _logger;
  private readonly SourceContext _source;

  public ExtractEventsCommandHandler(ILogger<ExtractEventsCommandHandler> logger, SourceContext source)
  {
    _logger = logger;
    _source = source;
  }

  public async Task<IReadOnlyCollection<EventEntity>> Handle(ExtractEventsCommand _, CancellationToken cancellationToken)
  {
    int total = await _source.Events.AsNoTracking().CountAsync(cancellationToken);
    List<EventEntity> events = new(capacity: total);

    int batchCount = total / BatchSize;
    if (total % BatchSize > 0)
    {
      batchCount++;
    }

    double percentage;
    for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
    {
      int skip = batchIndex * BatchSize;
      EventEntity[] batch = await _source.Events.AsNoTracking()
        .OrderBy(e => e.EventId)
        .Skip(skip)
        .Take(BatchSize)
        .ToArrayAsync(cancellationToken);
      events.AddRange(batch);

      percentage = events.Count / (double)total;
      _logger.LogInformation("[{Count}/{Total}] ({Percentage}) Extracted {EventCount} {EventText} from source database.",
        events.Count,
        total,
        percentage.ToString("P2"),
        batch.Length,
        batch.Length > 1 ? "events" : "event");
    }

    return events.AsReadOnly();
  }
}
