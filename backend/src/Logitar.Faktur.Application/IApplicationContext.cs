using Logitar.EventSourcing;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Application;

public interface IApplicationContext
{
  Actor Actor { get; }
  ActorId ActorId { get; }
}
