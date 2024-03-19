using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public ReplaceArticleCommandTests() : base()
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

  [Fact(DisplayName = "It should replace an existing article.")]
  public async Task It_should_replace_an_existing_article()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    long version = article.Version;
    await _articleRepository.SaveAsync(article);

    GtinUnit gtin = new("06038385904");
    article.Gtin = gtin;
    article.Update();
    await _articleRepository.SaveAsync(article);

    ReplaceArticlePayload payload = new(article.DisplayName.Value)
    {
      Gtin = null
    };
    ReplaceArticleCommand command = new(article.Id.ToGuid(), payload, version);
    Article? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(article.Id.ToGuid(), result.Id);

    Assert.Equal(gtin.Value, result.Gtin);
    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(payload.Description, result.Description);
  }

  [Fact(DisplayName = "It should return null when the article cannot be found.")]
  public async Task It_should_return_null_when_the_article_cannot_be_found()
  {
    ReplaceArticlePayload payload = new("PC POULET BBQ");
    ReplaceArticleCommand command = new(Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceArticlePayload payload = new(displayName: string.Empty);
    ReplaceArticleCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("NotEmptyValidator", Assert.Single(exception.Errors).ErrorCode);
  }
}
