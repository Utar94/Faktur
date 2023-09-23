using Logitar.EventSourcing;
using Logitar.Faktur.Application;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Web;

internal class HttpApplicationContext : IApplicationContext
{
  public Actor Actor => new()
  {
    Id = "fpion",
    Type = ActorType.User,
    DisplayName = "Francis Pion",
    EmailAddress = "francispion@hotmail.com",
    PictureUrl = "https://www.francispion.ca/assets/img/profile-img.jpg"
  };
  public ActorId ActorId => new(Actor.Id);
}
