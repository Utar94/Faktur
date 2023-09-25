using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class StoreServiceTests : IntegrationTests
{
  private readonly IBannerRepository bannerRepository;
  private readonly IStoreRepository storeRepository;
  private readonly IStoreService storeService;

  private readonly BannerAggregate banner;
  private readonly StoreAggregate store;

  public StoreServiceTests() : base()
  {
    bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
    storeService = ServiceProvider.GetRequiredService<IStoreService>();

    banner = new(new DisplayNameUnit("Maxi"), ApplicationContext.ActorId, BannerId.Parse("MAXI", "Id"));
    store = new(new DisplayNameUnit("Maxi Drummondville"), ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await bannerRepository.SaveAsync(banner);
    await storeRepository.SaveAsync(store);
  }

  [Fact(DisplayName = "CreateAsync: it should create the correct store.")]
  public async Task CreateAsync_it_should_create_the_correct_store()
  {
    CreateStorePayload payload = new()
    {
      Id = "  IGA  ",
      BannerId = $"  {banner.Id.Value}  ",
      Number = "08984",
      DisplayName = "  IGA Drummondville  ",
      Description = "    ",
      Address = new AddressPayload
      {
        Street = "  1870 boul Saint-Joseph  ",
        Locality = "  Drummondville  ",
        Region = "QC",
        PostalCode = " J2B 1R2 ",
        Country = "CA"
      },
      Phone = new PhonePayload
      {
        Number = "  819-472-1197  "
      }
    };

    AcceptedCommand command = await storeService.CreateAsync(payload);

    Assert.Equal(payload.Id.Trim(), command.AggregateId);
    Assert.True(command.AggregateVersion >= 1);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking()
      .Include(x => x.Banner)
      .SingleOrDefaultAsync(x => x.AggregateId == command.AggregateId);
    Assert.NotNull(store);
    Assert.Equal(command.AggregateId, store.AggregateId);
    Assert.Equal(command.AggregateVersion, store.Version);
    Assert.Equal(command.Actor.Id, store.CreatedBy);
    Assert.Equal(command.Actor.Id, store.UpdatedBy);
    AssertAreNear(command.Timestamp, AsUniversalTime(store.CreatedOn));
    AssertAreNear(command.Timestamp, AsUniversalTime(store.UpdatedOn));

    Assert.Equal(payload.Number.Trim(), store.Number);
    Assert.Equal(payload.DisplayName.Trim(), store.DisplayName);
    Assert.Null(store.Description);

    Assert.Equal(payload.Phone.Number.Trim(), store.PhoneNumber);

    Assert.Equal(payload.Address.Street.Trim(), store.AddressStreet);
    Assert.Equal(payload.Address.Locality.Trim(), store.AddressLocality);
    Assert.Equal(payload.Address.Region, store.AddressRegion);
    Assert.Equal(payload.Address.PostalCode.Trim(), store.AddressPostalCode);
    Assert.Equal(payload.Address.Country.Trim(), store.AddressCountry);
    Assert.Equal(PostalAddressHelper.Format(payload.Address), store.AddressFormatted);

    Assert.NotNull(store.Banner);
    Assert.Equal(banner.Id.Value, store.Banner.AggregateId);
  }

  [Fact(DisplayName = "CreateAsync: it should throw IdentifierAlreadyUsedException when the Gtin is already used.")]
  public async Task CreateAsync_it_should_throw_IdentifierAlreadyUsedException_when_the_Gtin_is_already_used()
  {
    CreateStorePayload payload = new()
    {
      Id = store.Id.Value,
      DisplayName = store.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<IdentifierAlreadyUsedException<StoreAggregate>>(async () => await storeService.CreateAsync(payload));
    Assert.Equal(store.Id.AggregateId, exception.Id);
    Assert.Equal(nameof(payload.Id), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task CreateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateStorePayload payload = new()
    {
      Address = new AddressPayload
      {
        Street = "1870 boul Saint-Joseph",
        Locality = "Drummondville",
        Region = "ZZ",
        Country = "CA"
      }
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await storeService.CreateAsync(payload));
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the correct store.")]
  public async Task DeleteAsync_it_should_delete_the_correct_store()
  {
    StoreAggregate store = new(new DisplayNameUnit("Metro Drummondville"), ApplicationContext.ActorId);
    await storeRepository.SaveAsync(store);

    AcceptedCommand command = await storeService.DeleteAsync(store.Id.Value);
    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    Assert.Null(await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == store.Id.Value));
    Assert.NotNull(await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.store.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task DeleteAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    string id = Guid.Empty.ToString();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await storeService.DeleteAsync(id)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct store by ID.")]
  public async Task ReadAsync_it_should_read_the_correct_store_by_id()
  {
    Store? store = await storeService.ReadAsync(id: this.store.Id.Value);
    Assert.NotNull(store);

    Assert.Equal(this.store.Id.Value, store.Id);
    Assert.Equal(this.store.Version, store.Version);
    Assert.Equal(ApplicationContext.Actor.Id, store.CreatedBy.Id);
    AssertAreNear(this.store.CreatedOn, store.CreatedOn);
    Assert.Equal(ApplicationContext.Actor.Id, store.UpdatedBy.Id);
    AssertAreNear(this.store.UpdatedOn, store.UpdatedOn);

    Assert.Equal(this.store.DisplayName.Value, store.DisplayName);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when no store are found.")]
  public async Task ReadAsync_it_should_return_null_when_no_store_are_found()
  {
    Assert.Null(await storeService.ReadAsync(id: Guid.Empty.ToString()));
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the correct store.")]
  public async Task ReplaceAsync_it_should_replace_the_correct_store()
  {
    long version = this.store.Version;

    this.store.DisplayName = new DisplayNameUnit("Maxi Drummondville (#08772)");
    this.store.Update(ApplicationContext.ActorId);
    await storeRepository.SaveAsync(this.store);

    ReplaceStorePayload payload = new()
    {
      BannerId = $"  {banner.Id.Value}  ",
      Number = "08772",
      DisplayName = "Maxi Drummondville",
      Description = "  Supermarché à proximité de la bibliothèque municipale et du terminus d'autobus.  ",
      Address = new AddressPayload
      {
        Street = "  1870 boul Saint-Joseph  ",
        Locality = "  Drummondville  ",
        Region = "QC",
        PostalCode = " J2B 1R2 ",
        Country = "CA"
      },
      Phone = new PhonePayload
      {
        CountryCode = "CA",
        Number = "  819-472-1197  ",
        Extension = "  12345  "
      }
    };

    AcceptedCommand command = await storeService.ReplaceAsync(this.store.Id.Value, payload, version);
    Assert.Equal(this.store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking()
      .Include(x => x.Banner)
      .SingleOrDefaultAsync(x => x.AggregateId == this.store.Id.Value);
    Assert.NotNull(store);
    Assert.Equal(8772, store.NumberNormalized);
    Assert.Equal("Maxi Drummondville (#08772)", store.DisplayName);
    Assert.Equal(payload.Description.Trim(), store.Description);

    Assert.Equal(payload.Address.Street.Trim(), store.AddressStreet);
    Assert.Equal(payload.Address.Locality.Trim(), store.AddressLocality);
    Assert.Equal(payload.Address.Region, store.AddressRegion);
    Assert.Equal(payload.Address.PostalCode.Trim(), store.AddressPostalCode);
    Assert.Equal(payload.Address.Country.Trim(), store.AddressCountry);
    Assert.Equal(PostalAddressHelper.Format(payload.Address), store.AddressFormatted);

    Assert.Equal(payload.Phone.CountryCode, store.PhoneCountryCode);
    Assert.Equal(payload.Phone.Number.Trim(), store.PhoneNumber);
    Assert.Equal(payload.Phone.Extension.Trim(), store.PhoneExtension);

    Assert.NotNull(store.Banner);
    Assert.Equal(banner.Id.Value, store.Banner.AggregateId);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task ReplaceAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    ReplaceStorePayload payload = new()
    {
      DisplayName = store.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await storeService.ReplaceAsync(id, payload)
    );
    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task ReplaceAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceStorePayload payload = new();

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await storeService.ReplaceAsync(store.Id.Value, payload));
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchStoresPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = new List<SearchTerm>()
        {
          new(Guid.Empty.ToString())
        }
      }
    };

    SearchResults<Store> stores = await storeService.SearchAsync(payload);

    Assert.Empty(stores.Results);
    Assert.Equal(0, stores.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    store.SetBanner(banner);
    store.Update(ApplicationContext.ActorId);

    StoreAggregate maxi08772 = new(new DisplayNameUnit("Maxi Drummondville"), ApplicationContext.ActorId, StoreId.Parse("MAXI-08772", "Id"))
    {
      Number = new StoreNumberUnit("08772"),
      Address = new AddressUnit("1870 boul Saint-Joseph", "Drummondville", "CA", "QC", "J2B 1R2"),
      Phone = new PhoneUnit("819-472-1197")
    };
    maxi08772.SetBanner(banner);
    maxi08772.Update(ApplicationContext.ActorId);

    StoreAggregate maxi08984 = new(new DisplayNameUnit("Maxi Drummondville St-Joseph"), ApplicationContext.ActorId, StoreId.Parse("MAXI-08984", "Id"))
    {
      Number = new StoreNumberUnit("08984"),
      Address = new AddressUnit("325 boul Saint-Joseph", "Drummondville", "CA", "QC", "J2C 8P7"),
      Phone = new PhoneUnit("819-477-4695")
    };
    maxi08984.SetBanner(banner);
    maxi08984.Update(ApplicationContext.ActorId);

    StoreAggregate maxi04561 = new(new DisplayNameUnit("Maxi Saint-Hyacinthe Casavant"), ApplicationContext.ActorId, StoreId.Parse("MAXI-04561", "Id"))
    {
      Number = new StoreNumberUnit("04561"),
      Address = new AddressUnit("2000 boul Casavant O", "Saint-Hyacinthe", "CA", "QC", "J2S 7K2"),
      Phone = new PhoneUnit("450-771-6601")
    };
    maxi04561.SetBanner(banner);
    maxi04561.Update(ApplicationContext.ActorId);

    StoreAggregate maxi04349 = new(new DisplayNameUnit("Maxi St-Hyacinthe Saint-Louis"), ApplicationContext.ActorId, StoreId.Parse("MAXI-04349", "Id"))
    {
      Number = new StoreNumberUnit("04349"),
      Address = new AddressUnit("15000 ave Saint-Louis", "Saint-Hyacinthe", "CA", "QC", "J2T 3E2"),
      Phone = new PhoneUnit("450-771-2737")
    };
    maxi04349.SetBanner(banner);
    maxi04349.Update(ApplicationContext.ActorId);

    StoreAggregate maxi06524 = new(new DisplayNameUnit("Maxi Sherbrooke Portland"), ApplicationContext.ActorId, StoreId.Parse("MAXI-06524", "Id"))
    {
      Number = new StoreNumberUnit("06524"),
      Address = new AddressUnit("3025 boul de Portland", "Sherbrooke", "CA", "QC", "J1L 2Y7"),
      Phone = new PhoneUnit("819-562-4041")
    };
    maxi06524.SetBanner(banner);
    maxi06524.Update(ApplicationContext.ActorId);

    StoreAggregate iga = new(new DisplayNameUnit("IGA Extra Marché Clément des Forges inc."), ApplicationContext.ActorId, StoreId.Parse("IGA-41910", "Id"))
    {
      Number = new StoreNumberUnit("41910"),
      Address = new AddressUnit("1910 boul St-Joseph", "Drummondville", "CA", "QC", "J2B 1R2"),
      Phone = new PhoneUnit("819-477-7700")
    };
    iga.Update(ApplicationContext.ActorId);

    StoreAggregate[] newStores = new[] { maxi08772, maxi08984, maxi04561, maxi04349, maxi06524, iga };
    await storeRepository.SaveAsync(newStores);

    StoreId[] ids = new[] { maxi08772.Id, maxi08984.Id, maxi04561.Id, maxi04349.Id, maxi06524.Id, iga.Id };
    SearchStoresPayload payload = new()
    {
      BannerId = banner.Id.Value,
      Id = new TextSearch
      {
        Terms = ids.Select(id => new SearchTerm(id.Value)).ToList(),
        Operator = SearchOperator.Or
      },
      Search = new TextSearch
      {
        Terms = new List<SearchTerm>
        {
          new("1870%"),
          new("%St-Joseph%"),
          new("+14507716601"),
          new("_4349")
        },
        Operator = SearchOperator.Or
      },
      Sort = new List<StoreSortOption>
      {
        new StoreSortOption(StoreSort.Number)
      },
      Skip = 1,
      Limit = 2
    };

    StoreAggregate[] expected = new[] { maxi08772, maxi08984, maxi04561, maxi04349 }.OrderBy(x => x.Number?.NormalizedValue)
      .Skip(payload.Skip).Take(payload.Limit).ToArray();

    SearchResults<Store> stores = await storeService.SearchAsync(payload);
    Assert.Equal(4, stores.Total);

    Assert.Equal(expected.Length, stores.Results.Count);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i].Id.Value, stores.Results[i].Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    string id = Guid.Empty.ToString();
    UpdateStorePayload payload = new();
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await storeService.UpdateAsync(id, payload)
    );

    Assert.Equal(id, exception.Id.Value);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task UpdateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateStorePayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await storeService.UpdateAsync(store.Id.Value, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the correct store.")]
  public async Task UpdateAsync_it_should_update_the_correct_store()
  {
    UpdateStorePayload payload = new()
    {
      BannerId = new Modification<string>(banner.Id.Value),
      Number = new Modification<string>("08984"),
      Description = new Modification<string>("  Supermarché à proximité de la bibliothèque municipale et du terminus d'autobus.  "),
      Address = new Modification<AddressPayload>(new()
      {
        Street = "1870 boul Saint-Joseph",
        Locality = "Drummondville",
        Region = "QC",
        PostalCode = "J2B 1R2",
        Country = "CA"
      }),
      Phone = new Modification<PhonePayload>(new()
      {
        Number = "819-472-1197"
      })
    };

    AcceptedCommand command = await storeService.UpdateAsync(this.store.Id.Value, payload);
    Assert.Equal(this.store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > this.store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking()
      .Include(x => x.Banner)
      .SingleOrDefaultAsync(x => x.AggregateId == this.store.Id.Value);
    Assert.NotNull(store);
    Assert.Equal(payload.Number.Value, store.Number);
    Assert.Equal(payload.Description.Value?.Trim(), store.Description);

    Assert.NotNull(payload.Address.Value);
    Assert.Equal(payload.Address.Value.Street, store.AddressStreet);
    Assert.Equal(payload.Address.Value.Locality, store.AddressLocality);
    Assert.Equal(payload.Address.Value.Region, store.AddressRegion);
    Assert.Equal(payload.Address.Value.PostalCode, store.AddressPostalCode);
    Assert.Equal(payload.Address.Value.Country, store.AddressCountry);
    Assert.Equal(PostalAddressHelper.Format(payload.Address.Value), store.AddressFormatted);

    Assert.NotNull(payload.Phone.Value);
    Assert.Equal(payload.Phone.Value.Number, store.PhoneNumber);

    Assert.NotNull(store.Banner);
    Assert.Equal(banner.Id.Value, store.Banner.AggregateId);
  }
}
