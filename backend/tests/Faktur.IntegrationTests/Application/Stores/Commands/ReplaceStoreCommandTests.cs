using Faktur.Application.Banners;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceStoreCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly StoreAggregate _store;

  public ReplaceStoreCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should replace an existing store.")]
  public async Task It_should_replace_an_existing_store()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"), ActorId);
    await _bannerRepository.SaveAsync(banner);

    long version = _store.Version;

    NumberUnit number = new("08872");
    _store.Number = number;
    _store.Update(ActorId);
    await _storeRepository.SaveAsync(_store);

    ReplaceStorePayload payload = new(_store.DisplayName.Value)
    {
      BannerId = banner.Id.ToGuid(),
      Number = null,
      Email = new EmailPayload("drummondville-08872@maxi.ca", isVerified: false)
    };
    ReplaceStoreCommand command = new(_store.Id.ToGuid(), payload, version);
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);

    Assert.Equal(_store.Id.ToGuid(), result.Id);
    Assert.Equal(version + 1, _store.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(_store.CreatedOn < _store.UpdatedOn);

    Assert.Equal(number.Value, result.Number);
    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(payload.Description, result.Description);
    Assert.Equal(payload.BannerId, result.Banner?.Id);

    Assert.NotNull(result.Email);
    Assert.Equal(payload.Email.Address, result.Email.Address);
    Assert.Equal(payload.Email.IsVerified, result.Email.IsVerified);
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    ReplaceStorePayload payload = new("Maxi Drummondville");
    ReplaceStoreCommand command = new(Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw BannerNotFoundException when the banner cannot be found.")]
  public async Task It_should_throw_BannerNotFoundException_when_the_banner_cannot_be_found()
  {
    ReplaceStorePayload payload = new("Maxi Drummondville")
    {
      BannerId = Guid.NewGuid()
    };
    ReplaceStoreCommand command = new(_store.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<BannerNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.BannerId, exception.BannerId);
    Assert.Equal(nameof(payload.BannerId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceStorePayload payload = new(displayName: string.Empty);
    ReplaceStoreCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.DisplayName), error.PropertyName);
  }
}
