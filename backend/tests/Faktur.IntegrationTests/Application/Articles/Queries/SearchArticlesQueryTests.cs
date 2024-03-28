using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchArticlesQueryTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public SearchArticlesQueryTests() : base()
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

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchArticlesPayload payload = new();
    SearchArticlesQuery query = new(payload);
    SearchResults<Article> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    ArticleAggregate notInIds = new(new DisplayNameUnit("SN CORNIC SUC"))
    {
      Gtin = new GtinUnit("06038309557")
    };
    notInIds.Update();
    ArticleAggregate notMatching = new(new DisplayNameUnit("CANTON BOUILLON"))
    {
      Gtin = new GtinUnit("05526622290")
    };
    notMatching.Update();
    ArticleAggregate article = new(new DisplayNameUnit("FARINE PIZZA"))
    {
      Gtin = new GtinUnit("06038317981")
    };
    article.Update();
    ArticleAggregate expected = new(new DisplayNameUnit("UNICO SAUCE PZZA"))
    {
      Gtin = new GtinUnit("06780000170")
    };
    expected.Update();
    await _articleRepository.SaveAsync([notInIds, notMatching, article, expected]);

    SearchArticlesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%60383%"));
    payload.Search.Terms.Add(new SearchTerm("%PZZA%"));
    payload.Sort.Add(new ArticleSortOption(ArticleSort.DisplayName));

    IEnumerable<ArticleAggregate> allArticles = await _articleRepository.LoadAsync();
    payload.Ids.AddRange(allArticles.Select(article => article.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchArticlesQuery query = new(payload);
    SearchResults<Article> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Article result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
