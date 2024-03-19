using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceStoreCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  public ReplaceStoreCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Stores.Table, FakturDb.Banners.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should replace an existing store.")]
  public async Task It_should_replace_an_existing_store()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    long version = store.Version;
    await _storeRepository.SaveAsync(store);

    NumberUnit number = new("08872");
    store.Number = number;
    store.Update();
    await _storeRepository.SaveAsync(store);

    ReplaceStorePayload payload = new(store.DisplayName.Value)
    {
      BannerId = banner.Id.ToGuid(),
      Number = null
    };
    ReplaceStoreCommand command = new(store.Id.ToGuid(), payload, version);
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(store.Id.ToGuid(), result.Id);

    Assert.Equal(number.Value, result.Number);
    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(payload.Description, result.Description);
    Assert.Equal(payload.BannerId, result.Banner?.Id);
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    ReplaceStorePayload payload = new("Maxi Drummondville");
    ReplaceStoreCommand command = new(Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Mediator.Send(command));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceStorePayload payload = new(displayName: string.Empty);
    ReplaceStoreCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
