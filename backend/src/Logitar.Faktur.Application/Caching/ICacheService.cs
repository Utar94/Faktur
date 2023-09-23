using Logitar.EventSourcing;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
}
