using Faktur.Contracts;
using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public UpdateArticleCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
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

  [Fact(DisplayName = "It should return null when the article cannot be found.")]
  public async Task It_should_return_null_when_the_article_cannot_be_found()
  {
    UpdateArticlePayload payload = new();
    UpdateArticleCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateArticlePayload payload = new()
    {
      Gtin = new Modification<string>("ABC123")
    };
    UpdateArticleCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("AllowedCharactersValidator", error.ErrorCode);
    Assert.Equal("Gtin.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing article.")]
  public async Task It_should_update_an_existing_article()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync(article);

    UpdateArticlePayload payload = new()
    {
      Gtin = new Modification<string>("06038385904")
    };
    UpdateArticleCommand command = new(article.Id.ToGuid(), payload);
    Article? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(article.Id.ToGuid(), result.Id);

    Assert.Equal(payload.Gtin.Value, result.Gtin);
  }
}
