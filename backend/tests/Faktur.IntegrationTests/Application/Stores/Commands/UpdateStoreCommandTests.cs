using Faktur.Contracts;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Logitar.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateStoreCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  public UpdateStoreCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    UpdateStorePayload payload = new();
    UpdateStoreCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Mediator.Send(command));
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
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"))
    {
      BannerId = banner.Id
    };
    store.Update();
    await _storeRepository.SaveAsync(store);

    UpdateStorePayload payload = new()
    {
      BannerId = new Modification<Guid?>(null),
      Number = new Modification<string>("08872")
    };
    UpdateStoreCommand command = new(store.Id.ToGuid(), payload);
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(store.Id.ToGuid(), result.Id);

    Assert.Equal(payload.Number.Value, result.Number);
    Assert.Null(result.Banner);
  }
}
