using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteBannerCommandTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public DeleteBannerCommandTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Banners.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing banner.")]
  public async Task It_should_delete_an_existing_banner()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    DeleteBannerCommand command = new(banner.Id.ToGuid());
    Banner? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(banner.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Banners.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the banner cannot be found.")]
  public async Task It_should_return_null_when_the_banner_cannot_be_found()
  {
    DeleteBannerCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }

  // TODO(fpion): Remmove Stores Banner
}
