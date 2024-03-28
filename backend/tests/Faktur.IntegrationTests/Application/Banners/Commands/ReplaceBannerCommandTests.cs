using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceBannerCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public ReplaceBannerCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  [Fact(DisplayName = "It should replace an existing banner.")]
  public async Task It_should_replace_an_existing_banner()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"), ActorId);
    long version = banner.Version;
    await _bannerRepository.SaveAsync(banner);

    DescriptionUnit description = new("Imbattable, point final");
    banner.Description = description;
    banner.Update(ActorId);
    await _bannerRepository.SaveAsync(banner);

    ReplaceBannerPayload payload = new(banner.DisplayName.Value)
    {
      Description = null
    };
    ReplaceBannerCommand command = new(banner.Id.ToGuid(), payload, version);
    Banner? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(banner.Id.ToGuid(), result.Id);
    Assert.Equal(version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.Equal(description.Value, result.Description);
    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(description.Value, result.Description);
  }

  [Fact(DisplayName = "It should return null when the banner cannot be found.")]
  public async Task It_should_return_null_when_the_banner_cannot_be_found()
  {
    ReplaceBannerPayload payload = new("MAXI");
    ReplaceBannerCommand command = new(Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceBannerPayload payload = new(displayName: string.Empty);
    ReplaceBannerCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
