using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Faktur.EntityFrameworkCore.Relational.Actors;

internal interface IActorService
{
  Task<IEnumerable<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
