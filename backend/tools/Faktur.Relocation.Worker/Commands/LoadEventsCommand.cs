using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker.Commands;

internal record LoadEventsCommand(IEnumerable<EventEntity> Events) : IRequest;

internal class LoadEventsCommandHandler : IRequestHandler<LoadEventsCommand>
{
  private const string ActorIdKey = "ActorId";
  private const int BatchSize = 1000;

  private readonly ActorId _actorId;
  private readonly IEventBus _bus;
  private readonly EventContext _context;
  private readonly IEventSerializer _eventSerializer;
  private readonly ILogger<LoadEventsCommandHandler> _logger;
  private readonly ISqlHelper _sqlHelper;

  public LoadEventsCommandHandler(
    IEventBus bus,
    IConfiguration configuration,
    EventContext context,
    IEventSerializer eventSerializer,
    ILogger<LoadEventsCommandHandler> logger,
    ISqlHelper sqlHelper)
  {
    string? actorId = configuration.GetValue<string>(ActorIdKey);
    if (string.IsNullOrWhiteSpace(actorId))
    {
      throw new InvalidOperationException($"The configuration '{ActorIdKey}' is required.");
    }
    _actorId = new ActorId(actorId);

    _bus = bus;
    _context = context;
    _eventSerializer = eventSerializer;
    _logger = logger;
    _sqlHelper = sqlHelper;
  }

  public async Task Handle(LoadEventsCommand command, CancellationToken cancellationToken)
  {
    IReadOnlySet<Guid> existingIds = await LoadExistingIdsAsync(cancellationToken);

    int total = command.Events.Count();
    int index = 0;
    double percentage;
    foreach (EventEntity @event in command.Events)
    {
      if (existingIds.Contains(@event.Id))
      {
        // TODO(fpion): UPDATE
      }
      else
      {
        Type eventType = @event.GetType();
        ICommand insert = _sqlHelper
          .InsertInto(
            EventDb.Events.Id,
            EventDb.Events.ActorId,
            EventDb.Events.IsDeleted,
            EventDb.Events.OccurredOn,
            EventDb.Events.Version,
            EventDb.Events.AggregateType,
            EventDb.Events.AggregateId,
            EventDb.Events.EventType,
            EventDb.Events.EventData)
          .Value(
            @event.Id,
            _actorId.Value,
            @event.IsDeleted,
            @event.OccurredOn,
            @event.Version,
            @event.AggregateType,
            @event.AggregateId,
            @event.EventType,
            @event.EventData)
          .Build();
        await _context.Database.ExecuteSqlRawAsync(insert.Text, insert.Parameters.ToArray(), cancellationToken);
      }

      DomainEvent change = _eventSerializer.Deserialize(@event);
      change.ActorId = _actorId;
      await _bus.PublishAsync(change, cancellationToken);

      index++;
      percentage = index / (double)total;
      _logger.LogInformation(
        "[{Index}/{Total}] ({Percentage}) Handled '{EventType}'.",
        index,
        total,
        percentage.ToString("P2"),
        @event.GetType());
    }
  }

  private async Task<IReadOnlySet<Guid>> LoadExistingIdsAsync(CancellationToken cancellationToken)
  {
    int total = await _context.Events.AsNoTracking().CountAsync(cancellationToken);
    List<Guid> existingIds = new(capacity: total);

    int batchCount = total / BatchSize;
    if (total % BatchSize > 0)
    {
      batchCount++;
    }

    double percentage;
    for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
    {
      int skip = batchIndex * BatchSize;
      Guid[] ids = await _context.Events.AsNoTracking()
        .OrderBy(e => e.EventId)
        .Select(e => e.Id)
        .Skip(skip)
        .Take(BatchSize)
        .ToArrayAsync(cancellationToken);
      existingIds.AddRange(ids);

      percentage = existingIds.Count / (double)total;
      _logger.LogInformation("[{Count}/{Total}] ({Percentage}) Retrieved {IdCount} event {IdText} from destination database.",
        existingIds.Count,
        total,
        percentage.ToString("P2"),
        ids.Length,
        ids.Length > 1 ? "IDs" : "ID");
    }

    return existingIds.ToHashSet();
  }
}
