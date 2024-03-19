using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
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

  [Fact(DisplayName = "It should create a new store.")]
  public async Task It_should_create_a_new_store()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    CreateStorePayload payload = new("Maxi Drummondville")
    {
      BannerId = banner.Id.ToGuid(),
      Number = "08872"
    };
    CreateStoreCommand command = new(payload);
    Store store = await Mediator.Send(command);

    Assert.Equal(2, store.Version);
    Assert.Equal(Actor, store.CreatedBy);
    Assert.Equal(Actor, store.UpdatedBy);

    Assert.Equal(payload.Number, store.Number);
    Assert.Equal(payload.DisplayName, store.DisplayName);
    Assert.Equal(payload.Description, store.Description);
    Assert.Equal(payload.BannerId, store.Banner?.Id);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateStorePayload payload = new(displayName: string.Empty);
    CreateStoreCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
