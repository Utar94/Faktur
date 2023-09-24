using Logitar.Faktur.Application.Articles;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class ArticleServiceTests : IntegrationTests
{
  private readonly IArticleRepository articleRepository;
  private readonly IArticleService articleService;

  private readonly ArticleAggregate article;

  public ArticleServiceTests() : base()
  {
    articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    articleService = ServiceProvider.GetRequiredService<IArticleService>();

    DisplayNameUnit displayName = new("NOTRE-DAME BRIE");
    GtinUnit gtin = new("006740000010");
    ArticleId id = new(gtin);
    article = new(displayName, ApplicationContext.ActorId, id)
    {
      Gtin = gtin
    };
    article.Update(ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await articleRepository.SaveAsync(article);
  }

  [Fact(DisplayName = "CreateAsync: it should create the correct article.")]
  public async Task CreateAsync_it_should_create_the_correct_article()
  {
    CreateArticlePayload payload = new()
    {
      Id = "    ",
      Gtin = "006740000018",
      DisplayName = "  AGRO CAMEMBERT  ",
      Description = "    "
    };

    AcceptedCommand command = await articleService.CreateAsync(payload);

    string[] values = command.AggregateId.Split('-');
    Assert.Equal(2, values.Length);
    Assert.Equal(payload.Gtin, values.First());
    AssertIsNear(AsUniversalTime(DateTimeOffset.FromUnixTimeSeconds(long.Parse(values.Last())).DateTime));

    Assert.True(command.AggregateVersion >= 1);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ArticleEntity? article = await FakturContext.Articles.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == command.AggregateId);
    Assert.NotNull(article);
    Assert.Equal(command.AggregateId, article.AggregateId);
    Assert.Equal(command.AggregateVersion, article.Version);
    Assert.Equal(command.Actor.Id, article.CreatedBy);
    Assert.Equal(command.Actor.Id, article.UpdatedBy);
    AssertAreNear(command.Timestamp, AsUniversalTime(article.CreatedOn));
    AssertAreNear(command.Timestamp, AsUniversalTime(article.UpdatedOn));

    Assert.Equal(payload.Gtin, article.Gtin);
    Assert.Equal(long.Parse(payload.Gtin), article.GtinNormalized);
    Assert.Equal(payload.DisplayName.Trim(), article.DisplayName);
    Assert.Null(article.Description);
  }

  [Fact(DisplayName = "CreateAsync: it should throw GtinAlreadyUsedException when the Gtin is already used.")]
  public async Task CreateAsync_it_should_throw_GtinAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    Assert.NotNull(article.Gtin);
    CreateArticlePayload payload = new()
    {
      Gtin = article.Gtin.Value,
      DisplayName = article.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<GtinAlreadyUsedException>(async () => await articleService.CreateAsync(payload));
    Assert.Equal(article.Gtin, exception.Gtin);
    Assert.Equal(nameof(payload.Gtin), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw IdentifierAlreadyUsedException when the Gtin is already used.")]
  public async Task CreateAsync_it_should_throw_IdentifierAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    CreateArticlePayload payload = new()
    {
      Id = article.Id.Value,
      DisplayName = article.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<IdentifierAlreadyUsedException<ArticleAggregate>>(async () => await articleService.CreateAsync(payload));
    Assert.Equal(article.Id.AggregateId, exception.Id);
    Assert.Equal(nameof(payload.Id), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task CreateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateArticlePayload payload = new()
    {
      Gtin = "ABC123"
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await articleService.CreateAsync(payload));
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the correct article.")]
  public async Task DeleteAsync_it_should_delete_the_correct_article()
  {
    GtinUnit gtin = new("006740000018");
    ArticleId id = new(gtin);
    ArticleAggregate article = new(new DisplayNameUnit("AGRO CAMEMBERT"), ApplicationContext.ActorId, id)
    {
      Gtin = gtin
    };
    await articleRepository.SaveAsync(article);

    AcceptedCommand command = await articleService.DeleteAsync(id.Value);
    Assert.Equal(id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > article.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    Assert.Null(await FakturContext.Articles.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == id.Value));
    Assert.NotNull(await FakturContext.Articles.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.article.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should throw AggregateNotFoundException when the article could not be found.")]
  public async Task DeleteAsync_it_should_throw_AggregateNotFoundException_when_the_article_could_not_be_found()
  {
    string id = Guid.Empty.ToString();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ArticleAggregate>>(
      async () => await articleService.DeleteAsync(id)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct article by GTIN.")]
  public async Task ReadAsync_it_should_read_the_correct_article_by_Gtin()
  {
    Assert.NotNull(this.article.Gtin);
    Article? article = await articleService.ReadAsync(gtin: this.article.Gtin.Value);
    Assert.NotNull(article);
    Assert.Equal(this.article.Id.Value, article.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct article by ID.")]
  public async Task ReadAsync_it_should_read_the_correct_article_by_id()
  {
    Article? article = await articleService.ReadAsync(id: this.article.Id.Value);
    Assert.NotNull(article);

    Assert.Equal(this.article.Id.Value, article.Id);
    Assert.Equal(this.article.Version, article.Version);
    Assert.Equal(ApplicationContext.Actor.Id, article.CreatedBy.Id);
    AssertAreNear(this.article.CreatedOn, article.CreatedOn);
    Assert.Equal(ApplicationContext.Actor.Id, article.UpdatedBy.Id);
    AssertAreNear(this.article.UpdatedOn, article.UpdatedOn);

    Assert.Equal(this.article.Gtin?.Value, article.Gtin);
    Assert.Equal(this.article.DisplayName.Value, article.DisplayName);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when no article are found.")]
  public async Task ReadAsync_it_should_return_null_when_no_article_are_found()
  {
    Assert.Null(await articleService.ReadAsync(id: Guid.Empty.ToString(), gtin: "006740000018"));
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many articles are found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_articles_are_found()
  {
    DisplayNameUnit displayName = new("AGRO CAMEMBERT");
    GtinUnit gtin = new("006740000018");
    ArticleId id = new(gtin);
    ArticleAggregate article = new(displayName, ApplicationContext.ActorId, id)
    {
      Gtin = gtin
    };
    article.Update(ApplicationContext.ActorId);
    await articleRepository.SaveAsync(article);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Article>>(
      async () => await articleService.ReadAsync(this.article.Id.Value, gtin.Value)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the correct article.")]
  public async Task ReplaceAsync_it_should_replace_the_correct_article()
  {
    long version = this.article.Version;

    this.article.DisplayName = new DisplayNameUnit("ANANAS");
    this.article.Update(ApplicationContext.ActorId);
    await articleRepository.SaveAsync(this.article);

    ReplaceArticlePayload payload = new()
    {
      Gtin = "006740000010",
      DisplayName = "NOTRE-DAME BRIE",
      Description = "  Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons.  "
    };

    AcceptedCommand command = await articleService.ReplaceAsync(this.article.Id.Value, payload, version);
    Assert.Equal(this.article.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ArticleEntity? article = await FakturContext.Articles.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.article.Id.Value);
    Assert.NotNull(article);
    Assert.Equal(payload.Gtin, article.Gtin);
    Assert.Equal("ANANAS", article.DisplayName);
    Assert.Equal(payload.Description.Trim(), article.Description);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw AggregateNotFoundException when the article could not be found.")]
  public async Task ReplaceAsync_it_should_throw_AggregateNotFoundException_when_the_article_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    ReplaceArticlePayload payload = new()
    {
      DisplayName = article.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ArticleAggregate>>(
      async () => await articleService.ReplaceAsync(id, payload)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw GtinAlreadyUsedException when the Gtin is already used.")]
  public async Task ReplaceAsync_it_should_throw_GtinAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    ArticleAggregate article = new(new DisplayNameUnit("AGRO CAMEMBERT"))
    {
      Gtin = new GtinUnit("006740000018")
    };
    article.Update(ApplicationContext.ActorId);
    await articleRepository.SaveAsync(article);

    ReplaceArticlePayload payload = new()
    {
      Gtin = article.Gtin.Value,
      DisplayName = this.article.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<GtinAlreadyUsedException>(
      async () => await articleService.ReplaceAsync(this.article.Id.Value, payload)
    );
    Assert.Equal(article.Gtin, exception.Gtin);
    Assert.Equal(nameof(payload.Gtin), exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task ReplaceAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceArticlePayload payload = new();

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await articleService.ReplaceAsync(article.Id.Value, payload));
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchArticlesPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = new List<SearchTerm>()
        {
          new(Guid.Empty.ToString())
        }
      }
    };

    SearchResults<Article> articles = await articleService.SearchAsync(payload);

    Assert.Empty(articles.Results);
    Assert.Equal(0, articles.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    ArticleAggregate asparagus = new(new DisplayNameUnit("ASPERGES"))
    {
      Gtin = new GtinUnit("4080")
    };
    ArticleAggregate babyBokChoy = new(new DisplayNameUnit("BABY BOK CHOY"));
    ArticleAggregate broccoli = new(new DisplayNameUnit("BROCOLI"))
    {
      Gtin = new GtinUnit("4060")
    };
    ArticleAggregate lemon = new(new DisplayNameUnit("CITRON"))
    {
      Gtin = new GtinUnit("4053")
    };
    ArticleAggregate pineapple = new(new DisplayNameUnit("ANANAS"))
    {
      Gtin = new GtinUnit("4029")
    };

    ArticleAggregate[] newArticles = new[] { asparagus, babyBokChoy, broccoli, lemon, pineapple };
    foreach (ArticleAggregate newArticle in newArticles)
    {
      newArticle.Update();
    }
    await articleRepository.SaveAsync(newArticles);

    ArticleId[] ids = new[] { article.Id, asparagus.Id, babyBokChoy.Id, broccoli.Id, pineapple.Id };
    SearchArticlesPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = ids.Select(id => new SearchTerm(id.Value)).ToList(),
        Operator = SearchOperator.Or
      },
      Search = new TextSearch
      {
        Terms = new List<SearchTerm>
        {
          new("%brie"),
          new("40__")
        },
        Operator = SearchOperator.Or
      },
      Sort = new List<ArticleSortOption>
      {
        new ArticleSortOption(ArticleSort.Gtin)
      },
      Skip = 1,
      Limit = 2
    };

    ArticleAggregate[] expected = new[] { article, asparagus, broccoli, pineapple }.OrderBy(x => x.Gtin?.NormalizedValue)
      .Skip(payload.Skip).Take(payload.Limit).ToArray();

    SearchResults<Article> articles = await articleService.SearchAsync(payload);
    Assert.Equal(4, articles.Total);

    Assert.Equal(expected.Length, articles.Results.Count);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i].Id.Value, articles.Results[i].Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the article could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_article_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    UpdateArticlePayload payload = new();
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ArticleAggregate>>(
      async () => await articleService.UpdateAsync(id, payload)
    );

    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw GtinAlreadyUsedException when the Gtin is already used.")]
  public async Task UpdateAsync_it_should_throw_GtinAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    ArticleAggregate article = new(new DisplayNameUnit("AGRO CAMEMBERT"))
    {
      Gtin = new GtinUnit("006740000018")
    };
    article.Update(ApplicationContext.ActorId);
    await articleRepository.SaveAsync(article);

    UpdateArticlePayload payload = new()
    {
      Gtin = new Modification<string>(article.Gtin.Value)
    };

    var exception = await Assert.ThrowsAsync<GtinAlreadyUsedException>(
      async () => await articleService.UpdateAsync(this.article.Id.Value, payload)
    );
    Assert.Equal(article.Gtin, exception.Gtin);
    Assert.Equal(nameof(payload.Gtin), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task UpdateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateArticlePayload payload = new()
    {
      Gtin = new Modification<string>("ABC123")
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await articleService.UpdateAsync(article.Id.Value, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the correct article.")]
  public async Task UpdateAsync_it_should_update_the_correct_article()
  {
    UpdateArticlePayload payload = new()
    {
      Description = new Modification<string>("  Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons.  ")
    };

    AcceptedCommand command = await articleService.UpdateAsync(this.article.Id.Value, payload);
    Assert.Equal(this.article.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > this.article.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ArticleEntity? article = await FakturContext.Articles.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.article.Id.Value);
    Assert.NotNull(article);
    Assert.Equal(payload.Description.Value?.Trim(), article.Description);
  }
}
