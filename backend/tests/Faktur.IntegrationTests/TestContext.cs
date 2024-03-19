using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Faktur;

internal record TestContext
{
  public User User { get; }

  public TestContext(User user)
  {
    User = user;
  }

  public static TestContext Create(Faker faker)
  {
    DateTime now = DateTime.UtcNow;
    User user = new(faker.Person.UserName)
    {
      Id = Guid.NewGuid(),
      Version = 3,
      CreatedOn = now,
      UpdatedOn = now,
      Email = new Email(faker.Person.Email),
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      FullName = faker.Person.FullName,
      Birthdate = faker.Person.DateOfBirth.ToUniversalTime(),
      Gender = faker.Person.Gender.ToString().ToLower(),
      Locale = new Locale(faker.Locale),
      Picture = faker.Person.Avatar,
      Website = $"https://www.{faker.Person.Website}/",
      AuthenticatedOn = now
    };

    Actor actor = new(user);
    user.CreatedBy = actor;
    user.UpdatedBy = actor;

    return new TestContext(user);
  }
}
