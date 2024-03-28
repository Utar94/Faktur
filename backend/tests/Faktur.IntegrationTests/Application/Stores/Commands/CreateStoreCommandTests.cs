using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational.Entities;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateStoreCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public CreateStoreCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  [Fact(DisplayName = "It should create a new store.")]
  public async Task It_should_create_a_new_store()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    CreateStorePayload payload = new("Maxi Drummondville")
    {
      BannerId = banner.Id.ToGuid(),
      Number = "08872",
      Phone = new PhonePayload(countryCode: "CA", "(819)-472-1197", extension: null, isVerified: true)
    };
    CreateStoreCommand command = new(payload);
    Store store = await Mediator.Send(command);

    Assert.Equal(2, store.Version);
    Assert.Equal(Actor, store.CreatedBy);
    Assert.Equal(Actor, store.UpdatedBy);
    Assert.True(store.CreatedOn < store.UpdatedOn);

    Assert.Equal(payload.Number, store.Number);
    Assert.Equal(payload.DisplayName, store.DisplayName);
    Assert.Equal(payload.Description, store.Description);
    Assert.Equal(payload.BannerId, store.Banner?.Id);

    Assert.NotNull(store.Phone);
    Assert.Equal(payload.Phone.CountryCode, store.Phone.CountryCode);
    Assert.Equal(payload.Phone.Number, store.Phone.Number);
    Assert.Equal(payload.Phone.Extension, store.Phone.Extension);
    Assert.Equal("+18194721197", store.Phone.E164Formatted);
    Assert.Equal(payload.Phone.IsVerified, store.Phone.IsVerified);

    StoreEntity? entity = await FakturContext.Stores.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(store.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateStorePayload payload = new(displayName: string.Empty);
    CreateStoreCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.DisplayName), error.PropertyName);
  }
}
