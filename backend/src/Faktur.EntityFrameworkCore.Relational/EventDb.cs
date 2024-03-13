using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;

namespace Faktur.EntityFrameworkCore.Relational;

internal static class EventDb // TODO(fpion): move to Logitar.EventSourcing and refactor Logitar.Identity
{
  public static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId ActorId = new(nameof(EventEntity.ActorId), Table);
    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
    public static readonly ColumnId EvenEventTypetId = new(nameof(EventEntity.EventType), Table);
    public static readonly ColumnId EventData = new(nameof(EventEntity.EventData), Table);
    public static readonly ColumnId EventId = new(nameof(EventEntity.EventId), Table);
    public static readonly ColumnId Id = new(nameof(EventEntity.Id), Table);
    public static readonly ColumnId IsDeleted = new(nameof(EventEntity.IsDeleted), Table);
    public static readonly ColumnId OccurredOn = new(nameof(EventEntity.OccurredOn), Table);
    public static readonly ColumnId Version = new(nameof(EventEntity.Version), Table);
  }
}
