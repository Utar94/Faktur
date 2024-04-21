using Faktur.ETL.Worker.Entities;
using Logitar;
using Logitar.Portal.Contracts.Actors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractActorsCommandHandler : IRequestHandler<ExtractActorsCommand, IEnumerable<Actor>>
{
  private readonly LegacyContext _context;

  public ExtractActorsCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Actor>> Handle(ExtractActorsCommand _, CancellationToken cancellationToken)
  {
    UserEntity[] users = await _context.Users.AsNoTracking().ToArrayAsync(cancellationToken);

    List<Actor> actors = new(capacity: users.Length);
    foreach (UserEntity user in users)
    {
      string? displayName = string.Join(' ', user.FirstName, user.LastName).CleanTrim() ?? user.UserName;
      if (displayName != null)
      {
        actors.Add(new Actor(displayName)
        {
          Id = user.Id,
          Type = ActorType.User,
          EmailAddress = user.Email,
          PictureUrl = user.Picture
        });
      }
    }

    return actors.AsReadOnly();
  }
}
