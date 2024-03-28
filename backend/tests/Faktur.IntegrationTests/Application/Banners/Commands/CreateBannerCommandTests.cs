using Faktur.Contracts.Banners;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateBannerCommandTests : IntegrationTests
{
  public CreateBannerCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new banner.")]
  public async Task It_should_create_a_new_banner()
  {
    CreateBannerPayload payload = new("MAXI");
    CreateBannerCommand command = new(payload);
    Banner banner = await Mediator.Send(command);

    Assert.Equal(1, banner.Version);
    Assert.Equal(Actor, banner.CreatedBy);
    Assert.Equal(Actor, banner.UpdatedBy);
    Assert.Equal(banner.CreatedOn, banner.UpdatedOn);

    Assert.Equal(payload.DisplayName, banner.DisplayName);
    Assert.Equal(payload.Description, banner.Description);

    BannerEntity? entity = await FakturContext.Banners.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(banner.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateBannerPayload payload = new(displayName: string.Empty);
    CreateBannerCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.DisplayName), error.PropertyName);
  }
}
