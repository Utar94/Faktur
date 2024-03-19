using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Portal.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadArticleQueryTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public ReadArticleQueryTests() : base()
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
    ReadArticleQuery query = new(Guid.NewGuid(), Gtin: "06038385904");
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the article when it is found by GTIN.")]
  public async Task It_should_return_the_article_when_it_is_found_by_Gtin()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"))
    {
      Gtin = new GtinUnit("06038385904")
    };
    article.Update();
    await _articleRepository.SaveAsync(article);

    ReadArticleQuery query = new(Id: null, article.Gtin.Value);
    Article? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(article.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should return the article when it is found by ID.")]
  public async Task It_should_return_the_article_when_it_is_found_by_Id()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync(article);

    ReadArticleQuery query = new(article.Id.ToGuid(), Gtin: null);
    Article? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(article.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple articles are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_articles_are_found()
  {
    ArticleAggregate article1 = new(new DisplayNameUnit("PC POULET BBQ"));
    ArticleAggregate article2 = new(new DisplayNameUnit("PC POULET BBQ"))
    {
      Gtin = new GtinUnit("06038385904")
    };
    article2.Update();
    await _articleRepository.SaveAsync([article1, article2]);
    ReadArticleQuery query = new(article1.Id.ToGuid(), article2.Gtin.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Article>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
