using Logitar.Faktur.Application.Exceptions;
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

    banner = new(new DisplayNameUnit("MAXI"), ApplicationContext.ActorId, BannerId.Parse("MAXI", "Id"));
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
      Id = "  IGA  ",
      DisplayName = "  IGA EXTRA  ",
      Description = "    "
    };

    AcceptedCommand command = await bannerService.CreateAsync(payload);

    Assert.Equal(payload.Id.Trim(), command.AggregateId);
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

  [Fact(DisplayName = "CreateAsync: it should throw IdentifierAlreadyUsedException when the Gtin is already used.")]
  public async Task CreateAsync_it_should_throw_IdentifierAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    CreateBannerPayload payload = new()
    {
      Id = banner.Id.Value,
      DisplayName = banner.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<IdentifierAlreadyUsedException<BannerAggregate>>(async () => await bannerService.CreateAsync(payload));
    Assert.Equal(banner.Id.AggregateId, exception.Id);
    Assert.Equal(nameof(payload.Id), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task CreateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateBannerPayload payload = new();

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await bannerService.CreateAsync(payload));
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the correct banner.")]
  public async Task DeleteAsync_it_should_delete_the_correct_banner()
  {
    BannerId id = BannerId.Parse("METRO", "Id");
    BannerAggregate banner = new(new DisplayNameUnit("METRO INC."), ApplicationContext.ActorId, id);
    await bannerRepository.SaveAsync(banner);

    AcceptedCommand command = await bannerService.DeleteAsync(id.Value);
    Assert.Equal(id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > banner.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    Assert.Null(await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == id.Value));
    Assert.NotNull(await FakturContext.Banners.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.banner.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should throw AggregateNotFoundException when the banner could not be found.")]
  public async Task DeleteAsync_it_should_throw_AggregateNotFoundException_when_the_banner_could_not_be_found()
  {
    string id = Guid.Empty.ToString();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<BannerAggregate>>(
      async () => await bannerService.DeleteAsync(id)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct banner by ID.")]
  public async Task ReadAsync_it_should_read_the_correct_banner_by_id()
  {
    Banner? banner = await bannerService.ReadAsync(id: this.banner.Id.Value);
    Assert.NotNull(banner);

    Assert.Equal(this.banner.Id.Value, banner.Id);
    Assert.Equal(this.banner.Version, banner.Version);
    Assert.Equal(ApplicationContext.Actor.Id, banner.CreatedBy.Id);
    AssertAreNear(this.banner.CreatedOn, banner.CreatedOn);
    Assert.Equal(ApplicationContext.Actor.Id, banner.UpdatedBy.Id);
    AssertAreNear(this.banner.UpdatedOn, banner.UpdatedOn);

    Assert.Equal(this.banner.DisplayName.Value, banner.DisplayName);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when no banner are found.")]
  public async Task ReadAsync_it_should_return_null_when_no_banner_are_found()
  {
    Assert.Null(await bannerService.ReadAsync(id: Guid.Empty.ToString()));
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

  [Fact(DisplayName = "ReplaceAsync: it should throw AggregateNotFoundException when the banner could not be found.")]
  public async Task ReplaceAsync_it_should_throw_AggregateNotFoundException_when_the_banner_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    ReplaceBannerPayload payload = new()
    {
      DisplayName = banner.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<BannerAggregate>>(
      async () => await bannerService.ReplaceAsync(id, payload)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task ReplaceAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceBannerPayload payload = new();

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await bannerService.ReplaceAsync(banner.Id.Value, payload));
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchBannersPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = new List<SearchTerm>()
        {
          new(Guid.Empty.ToString())
        }
      }
    };

    SearchResults<Banner> banners = await bannerService.SearchAsync(payload);

    Assert.Empty(banners.Results);
    Assert.Equal(0, banners.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    BannerAggregate iga = new(new DisplayNameUnit("IGA EXTRA"));
    BannerAggregate loblaws = new(new DisplayNameUnit("LOBLAWS"));
    BannerAggregate metro = new(new DisplayNameUnit("METRO INC."));
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

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the banner could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_banner_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    UpdateBannerPayload payload = new();
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<BannerAggregate>>(
      async () => await bannerService.UpdateAsync(id, payload)
    );

    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task UpdateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateBannerPayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await bannerService.UpdateAsync(banner.Id.Value, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the correct banner.")]
  public async Task UpdateAsync_it_should_update_the_correct_banner()
  {
    UpdateBannerPayload payload = new()
    {
      Description = new Modification<string>("  Maxi est une bannière québécoise de supermarchés à rabais. Fondée en 1984 par Provigo, elle appartient depuis 1998 à Loblaw.  ")
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
