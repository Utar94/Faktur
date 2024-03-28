using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  public CreateArticleCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
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
    Assert.True(article.CreatedOn < article.UpdatedOn);

    Assert.Equal(payload.Gtin, article.Gtin);
    Assert.Equal(payload.DisplayName, article.DisplayName);
    Assert.Equal(payload.Description, article.Description);

    ArticleEntity? entity = await FakturContext.Articles.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(article.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw GtinAlreadyUsedException when the Gtin is already used.")]
  public async Task It_should_throw_GtinAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"), ActorId)
    {
      Gtin = new GtinUnit("06038385904")
    };
    article.Update(ActorId);
    await _articleRepository.SaveAsync(article);

    CreateArticlePayload payload = new(article.DisplayName.Value)
    {
      Gtin = article.Gtin.Value
    };
    CreateArticleCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<GtinAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Gtin, exception.Gtin);
    Assert.Equal(nameof(payload.Gtin), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateArticlePayload payload = new(displayName: string.Empty);
    CreateArticleCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
