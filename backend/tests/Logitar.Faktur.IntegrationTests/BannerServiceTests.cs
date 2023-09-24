using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class BannerServiceTests : IntegrationTests
{
  private readonly IBannerRepository bannerRepository;
  private readonly IBannerService bannerService;

  private readonly BannerAggregate banner;

  public BannerServiceTests() : base()
  {
    bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    bannerService = ServiceProvider.GetRequiredService<IBannerService>();

    banner = new(new DisplayNameUnit("MAXI"), ApplicationContext.ActorId);
    banner.Update(ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await bannerRepository.SaveAsync(banner);
  }

  [Fact(DisplayName = "CreateAsync: it should create the correct banner.")]
  public async Task CreateAsync_it_should_create_the_correct_banner()
  {
    CreateBannerPayload payload = new()
    {
      Id = "    ",
      DisplayName = "  IGA  ",
      Description = "    "
    };

    AcceptedCommand command = await bannerService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, new AggregateId(command.AggregateId).ToGuid());
    Assert.True(command.AggregateVersion >= 1);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    BannerEntity? banner = await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == command.AggregateId);
    Assert.NotNull(banner);
    Assert.Equal(command.AggregateId, banner.AggregateId);
    Assert.Equal(command.AggregateVersion, banner.Version);
    Assert.Equal(command.Actor.Id, banner.CreatedBy);
    Assert.Equal(command.Actor.Id, banner.UpdatedBy);
    AssertAreNear(command.Timestamp, AsUniversalTime(banner.CreatedOn));
    AssertAreNear(command.Timestamp, AsUniversalTime(banner.UpdatedOn));

    Assert.Equal(payload.DisplayName.Trim(), banner.DisplayName);
    Assert.Null(banner.Description);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the correct banner.")]
  public async Task DeleteAsync_it_should_delete_the_correct_banner()
  {
    BannerAggregate banner = new(new DisplayNameUnit("IGA"), ApplicationContext.ActorId);
    await bannerRepository.SaveAsync(banner);

    AcceptedCommand command = await bannerService.DeleteAsync(banner.Id.Value);
    Assert.Equal(banner.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > banner.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    Assert.Null(await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == banner.Id.Value));
    Assert.NotNull(await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.banner.Id.Value));
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct banner.")]
  public async Task ReadAsync_it_should_read_the_correct_banner()
  {
    Banner? banner = await bannerService.ReadAsync(this.banner.Id.Value);
    Assert.NotNull(banner);

    Assert.Equal(this.banner.Id.Value, banner.Id);
    Assert.Equal(this.banner.Version, banner.Version);
    Assert.Equal(ApplicationContext.Actor.Id, banner.CreatedBy.Id);
    AssertAreNear(this.banner.CreatedOn, banner.CreatedOn);
    Assert.Equal(ApplicationContext.Actor.Id, banner.UpdatedBy.Id);
    AssertAreNear(this.banner.UpdatedOn, banner.UpdatedOn);

    Assert.Equal(this.banner.DisplayName.Value, banner.DisplayName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the correct banner.")]
  public async Task ReplaceAsync_it_should_replace_the_correct_banner()
  {
    long version = this.banner.Version;

    this.banner.DisplayName = new DisplayNameUnit("MAXI INC.");
    this.banner.Update(ApplicationContext.ActorId);
    await bannerRepository.SaveAsync(this.banner);

    ReplaceBannerPayload payload = new()
    {
      DisplayName = "MAXI",
      Description = "  Maxi est une bannière québécoise de supermarchés à rabais. Fondée en 1984 par Provigo, elle appartient depuis 1998 à Loblaw.  "
    };

    AcceptedCommand command = await bannerService.ReplaceAsync(this.banner.Id.Value, payload, version);
    Assert.Equal(this.banner.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    BannerEntity? banner = await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.banner.Id.Value);
    Assert.NotNull(banner);
    Assert.Equal("MAXI INC.", banner.DisplayName);
    Assert.Equal(payload.Description.Trim(), banner.Description);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    BannerAggregate iga = new(new DisplayNameUnit("IGA"));
    BannerAggregate loblaws = new(new DisplayNameUnit("LOBLAWS"));
    BannerAggregate metro = new(new DisplayNameUnit("METRO"));
    BannerAggregate provigo = new(new DisplayNameUnit("PROVIGO"));
    BannerAggregate superC = new(new DisplayNameUnit("SUPER C"));

    BannerAggregate[] newBanners = new[] { iga, loblaws, metro, provigo, superC };
    foreach (BannerAggregate newBanner in newBanners)
    {
      newBanner.Update();
    }

    await bannerRepository.SaveAsync(newBanners);

    BannerId[] ids = new[] { banner.Id, iga.Id, loblaws.Id, metro.Id, superC.Id };
    SearchBannersPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = ids.Select(id => new SearchTerm(id.Value)).ToList(),
        Operator = SearchOperator.Or
      },
      Search = new TextSearch
      {
        Terms = new List<SearchTerm>
        {
          new("%a%"),
          new("%o%")
        },
        Operator = SearchOperator.Or
      },
      Sort = new List<BannerSortOption>
      {
        new BannerSortOption(BannerSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    BannerAggregate[] expected = new[] { banner, iga, loblaws, metro }.OrderBy(x => x.DisplayName.Value)
      .Skip(payload.Skip).Take(payload.Limit).ToArray();

    SearchResults<Banner> banners = await bannerService.SearchAsync(payload);
    Assert.Equal(4, banners.Total);

    Assert.Equal(expected.Length, banners.Results.Count);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i].Id.Value, banners.Results[i].Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should update the correct banner.")]
  public async Task UpdateAsync_it_should_update_the_correct_banner()
  {
    UpdateBannerPayload payload = new()
    {
      Description = new Modification<string>("  Metro inc. est une entreprise de distribution alimentaire et pharmaceutique au Canada.  ")
    };

    AcceptedCommand command = await bannerService.UpdateAsync(this.banner.Id.Value, payload);
    Assert.Equal(this.banner.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > this.banner.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    BannerEntity? banner = await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.banner.Id.Value);
    Assert.NotNull(banner);
    Assert.Equal(payload.Description.Value?.Trim(), banner.Description);
  }
}
