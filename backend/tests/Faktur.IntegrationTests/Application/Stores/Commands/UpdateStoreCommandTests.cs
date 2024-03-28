using Faktur.Application.Banners;
using Faktur.Contracts;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateStoreCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly BannerAggregate _banner;
  private readonly StoreAggregate _store;

  public UpdateStoreCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _banner = new(new DisplayNameUnit("MAXI"), ActorId);
    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId)
    {
      BannerId = _banner.Id
    };
    _store.Update(ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _bannerRepository.SaveAsync(_banner);
    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    UpdateStorePayload payload = new();
    UpdateStoreCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw BannerNotFoundException when the banner cannot be found.")]
  public async Task It_should_throw_BannerNotFoundException_when_the_banner_cannot_be_found()
  {
    UpdateStorePayload payload = new()
    {
      BannerId = new Modification<Guid?>(Guid.NewGuid())
    };
    UpdateStoreCommand command = new(_store.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<BannerNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.BannerId.Value, exception.AggregateId.ToGuid());
    Assert.Equal(nameof(payload.BannerId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateStorePayload payload = new()
    {
      Number = new Modification<string>(RandomStringGenerator.GetString(NumberUnit.MaximumLength + 1))
    };
    UpdateStoreCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Number.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing store.")]
  public async Task It_should_update_an_existing_store()
  {
    UpdateStorePayload payload = new()
    {
      BannerId = new Modification<Guid?>(null),
      Number = new Modification<string>("08872"),
      Address = new Modification<AddressPayload>(new AddressPayload("1870 Bd Saint-Joseph", "Drummondville", "J2B 1R2", "QC", "CA", isVerified: true))
    };
    UpdateStoreCommand command = new(_store.Id.ToGuid(), payload);
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);

    Assert.Equal(_store.Id.ToGuid(), result.Id);
    Assert.Equal(3, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(_store.CreatedOn < _store.UpdatedOn);

    Assert.Equal(payload.Number.Value, result.Number);
    Assert.Null(result.Banner);

    Assert.NotNull(result.Address);
    Assert.NotNull(payload.Address.Value);
    Assert.Equal(payload.Address.Value.Street, result.Address.Street);
    Assert.Equal(payload.Address.Value.Locality, result.Address.Locality);
    Assert.Equal(payload.Address.Value.PostalCode, result.Address.PostalCode);
    Assert.Equal(payload.Address.Value.Region, result.Address.Region);
    Assert.Equal(payload.Address.Value.Country, result.Address.Country);
    Assert.Equal("1870 Bd Saint-Joseph\r\nDrummondville QC J2B 1R2\r\nCA", result.Address.Formatted);
    Assert.Equal(payload.Address.Value.IsVerified, result.Address.IsVerified);
  }
}
