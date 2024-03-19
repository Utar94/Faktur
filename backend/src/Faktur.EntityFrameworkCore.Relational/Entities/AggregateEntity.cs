using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal abstract class AggregateEntity
{
  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent @event)
  {
    AggregateId = @event.AggregateId.Value;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event);
  }

  public virtual IEnumerable<ActorId> GetActorIds() => [new(CreatedBy), new(UpdatedBy)];

  protected void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }

  public override bool Equals(object? obj) => obj is AggregateEntity entity && entity.GetType().Equals(GetType()) && entity.AggregateId == AggregateId;
  public override int GetHashCode() => HashCode.Combine(GetType(), AggregateId);
  public override string ToString() => $"{GetType()} (Id={AggregateId})";
}
