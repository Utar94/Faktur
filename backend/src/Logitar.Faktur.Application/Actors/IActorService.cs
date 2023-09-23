using Logitar.EventSourcing;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Application.Actors;

public interface IActorService
{
  Task<IEnumerable<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
