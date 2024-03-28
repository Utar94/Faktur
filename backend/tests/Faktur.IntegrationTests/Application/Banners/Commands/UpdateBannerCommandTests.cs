using Faktur.Contracts;
using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateBannerCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public UpdateBannerCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  [Fact(DisplayName = "It should return null when the banner cannot be found.")]
  public async Task It_should_return_null_when_the_banner_cannot_be_found()
  {
    UpdateBannerPayload payload = new();
    UpdateBannerCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateBannerPayload payload = new()
    {
      DisplayName = RandomStringGenerator.GetString(DisplayNameUnit.MaximumLength + 1)
    };
    UpdateBannerCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.DisplayName), error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing banner.")]
  public async Task It_should_update_an_existing_banner()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"), ActorId);
    await _bannerRepository.SaveAsync(banner);

    UpdateBannerPayload payload = new()
    {
      Description = new Modification<string>("Imbattable, point final")
    };
    UpdateBannerCommand command = new(banner.Id.ToGuid(), payload);
    Banner? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(banner.Id.ToGuid(), result.Id);
    Assert.Equal(banner.Version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.Equal(payload.Description.Value, result.Description);
  }
}
