using Bogus;
using Logitar.EventSourcing;
using Logitar.Faktur.Application;
using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur;

internal class TestApplicationContext : IApplicationContext
{
  public Actor Actor { get; }
  public ActorId ActorId => new(Actor.Id);

  public TestApplicationContext(Person person)
  {
    Actor = new()
    {
      Id = person.UserName,
      Type = ActorType.User,
      DisplayName = person.FullName,
      EmailAddress = person.Email,
      PictureUrl = person.Avatar
    };
  }
}
