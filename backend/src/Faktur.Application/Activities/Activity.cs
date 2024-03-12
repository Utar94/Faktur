using Logitar.EventSourcing;

namespace Faktur.Application.Activities;

public abstract record Activity : IActivity
{
  public ActorId ActorId { get; } // TODO(fpion): Authentication
}
