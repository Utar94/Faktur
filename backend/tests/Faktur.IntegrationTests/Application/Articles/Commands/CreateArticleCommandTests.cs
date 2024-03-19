using Faktur.Contracts.Articles;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateArticleCommandTests : IntegrationTests
{
  public CreateArticleCommandTests() : base()
  {
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Articles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should create a new article.")]
  public async Task It_should_create_a_new_article()
  {
    CreateArticlePayload payload = new("PC POULET BBQ")
    {
      Gtin = "06038385904"
    };
    CreateArticleCommand command = new(payload);
    Article article = await Mediator.Send(command);

    Assert.Equal(2, article.Version);
    Assert.Equal(Actor, article.CreatedBy);
    Assert.Equal(Actor, article.UpdatedBy);

    Assert.Equal(payload.Gtin, article.Gtin);
    Assert.Equal(payload.DisplayName, article.DisplayName);
    Assert.Equal(payload.Description, article.Description);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateArticlePayload payload = new(displayName: string.Empty);
    CreateArticleCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("NotEmptyValidator", Assert.Single(exception.Errors).ErrorCode);
  }
}
