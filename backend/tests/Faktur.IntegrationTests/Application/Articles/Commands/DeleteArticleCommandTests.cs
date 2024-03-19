using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public DeleteArticleCommandTests() : base()
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

  [Fact(DisplayName = "It should delete an existing article.")]
  public async Task It_should_delete_an_existing_article()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync(article);

    DeleteArticleCommand command = new(article.Id.ToGuid());
    Article? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(article.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Articles.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the article cannot be found.")]
  public async Task It_should_return_null_when_the_article_cannot_be_found()
  {
    DeleteArticleCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }

  // TODO(fpion): Delete Article Products
}
