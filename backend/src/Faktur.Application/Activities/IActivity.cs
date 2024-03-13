using Logitar.EventSourcing;

namespace Faktur.Application.Activities;

public interface IActivity
{
  ActorId ActorId { get; }

  void Contextualize(ActivityContext context);
}
