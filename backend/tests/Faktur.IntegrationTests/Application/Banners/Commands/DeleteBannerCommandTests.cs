using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteBannerCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly BannerAggregate _banner;

  public DeleteBannerCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _banner = new BannerAggregate(new DisplayNameUnit("MAXI"));
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

    await _bannerRepository.SaveAsync(_banner);
  }

  [Fact(DisplayName = "It should delete an existing banner.")]
  public async Task It_should_delete_an_existing_banner()
  {
    DeleteBannerCommand command = new(_banner.Id.ToGuid());
    Banner? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(_banner.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Banners.ToArrayAsync());
  }

  [Fact(DisplayName = "It should remove the store banner.")]
  public async Task It_should_remove_the_store_banner()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId)
    {
      BannerId = _banner.Id,
      Number = new NumberUnit("08872")
    };
    store.Update(ActorId);
    await _storeRepository.SaveAsync(store);

    DeleteBannerCommand command = new(_banner.Id.ToGuid());
    _ = await Mediator.Send(command);

    store = (await _storeRepository.LoadAsync(store.Id))!;
    Assert.NotNull(store);
    Assert.Null(store.BannerId);
  }

  [Fact(DisplayName = "It should return null when the banner cannot be found.")]
  public async Task It_should_return_null_when_the_banner_cannot_be_found()
  {
    DeleteBannerCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
