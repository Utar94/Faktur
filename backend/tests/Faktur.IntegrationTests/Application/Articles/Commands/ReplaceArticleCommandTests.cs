using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Articles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceArticleCommandTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;

  private readonly ArticleAggregate _article;

  public ReplaceArticleCommandTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();

    _article = new(new DisplayNameUnit("PC POULET BBQ"), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _articleRepository.SaveAsync(_article);
  }

  [Fact(DisplayName = "It should replace an existing article.")]
  public async Task It_should_replace_an_existing_article()
  {
    long version = _article.Version;

    GtinUnit gtin = new("06038385904");
    _article.Gtin = gtin;
    _article.Update(ActorId);
    await _articleRepository.SaveAsync(_article);

    ReplaceArticlePayload payload = new(_article.DisplayName.Value)
    {
      Gtin = null
    };
    ReplaceArticleCommand command = new(_article.Id.ToGuid(), payload, version);
    Article? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(_article.Id.ToGuid(), result.Id);
    Assert.Equal(version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

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

  [Fact(DisplayName = "It should throw GtinAlreadyUsedException when the Gtin is already used.")]
  public async Task It_should_throw_GtinAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    _article.Gtin = new GtinUnit("06038385904");
    _article.Update(ActorId);
    ArticleAggregate article = new(new DisplayNameUnit("PC POULET BBQ"));
    await _articleRepository.SaveAsync([_article, article]);

    ReplaceArticlePayload payload = new(article.DisplayName.Value)
    {
      Gtin = _article.Gtin.Value
    };
    ReplaceArticleCommand command = new(article.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<GtinAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Gtin, exception.Gtin);
    Assert.Equal(nameof(payload.Gtin), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceArticlePayload payload = new(displayName: string.Empty);
    ReplaceArticleCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
