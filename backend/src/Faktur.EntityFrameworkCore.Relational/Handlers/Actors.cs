using Faktur.Application.Actors;
using Faktur.Application.Caching;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Handlers;

internal static class Actors
{
  public class UserSignedInEventHandler : INotificationHandler<UserSignedInEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly FakturContext _context;

    public UserSignedInEventHandler(ICacheService cacheService, FakturContext context)
    {
      _cacheService = cacheService;
      _context = context;
    }

    public async Task Handle(UserSignedInEvent @event, CancellationToken cancellationToken)
    {
      User user = @event.User;
      ActorId id = new(user.Id);
      string actorId = id.Value;
      ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == actorId, cancellationToken);
      if (actor == null)
      {
        actor = new(user);

        _context.Actors.Add(actor);
      }
      else
      {
        actor.Update(user);
      }

      await _context.SaveChangesAsync(cancellationToken);

      _cacheService.RemoveActor(id);
    }
  }

  public class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly FakturContext _context;

    public UserUpdatedEventHandler(ICacheService cacheService, FakturContext context)
    {
      _cacheService = cacheService;
      _context = context;
    }

    public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
    {
      User user = @event.User;
      ActorId id = new(user.Id);
      string actorId = id.Value;
      ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == actorId, cancellationToken);
      if (actor == null)
      {
        actor = new(user);

        _context.Actors.Add(actor);
      }
      else
      {
        actor.Update(user);
      }

      await _context.SaveChangesAsync(cancellationToken);

      _cacheService.RemoveActor(id);
    }
  }
}
