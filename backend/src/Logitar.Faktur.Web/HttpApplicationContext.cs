using Logitar.EventSourcing;
using Logitar.Faktur.Application;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Web;

internal class HttpApplicationContext : IApplicationContext
{
  public Actor Actor => new();
  public ActorId ActorId => new(Actor.Id);
}
