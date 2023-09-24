using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class StoreServiceTests : IntegrationTests
{
  private readonly IStoreRepository storeRepository;
  private readonly IStoreService storeService;

  private readonly StoreAggregate store;

  public StoreServiceTests() : base()
  {
    storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
    storeService = ServiceProvider.GetRequiredService<IStoreService>();

    store = new(new DisplayNameUnit("Maxi Drummondville"), ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await storeRepository.SaveAsync(store);
  }

  [Fact(DisplayName = "CreateAsync: it should create the correct store.")]
  public async Task CreateAsync_it_should_create_the_correct_store()
  {
    CreateStorePayload payload = new()
    {
      Id = "  IGA  ",
      DisplayName = "  IGA Drummondville  ",
      Description = "    "
    };

    AcceptedCommand command = await storeService.CreateAsync(payload);

    Assert.Equal(payload.Id.Trim(), command.AggregateId);
    Assert.True(command.AggregateVersion >= 1);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == command.AggregateId);
    Assert.NotNull(store);
    Assert.Equal(command.AggregateId, store.AggregateId);
    Assert.Equal(command.AggregateVersion, store.Version);
    Assert.Equal(command.Actor.Id, store.CreatedBy);
    Assert.Equal(command.Actor.Id, store.UpdatedBy);
    AssertAreNear(command.Timestamp, AsUniversalTime(store.CreatedOn));
    AssertAreNear(command.Timestamp, AsUniversalTime(store.UpdatedOn));

    Assert.Equal(payload.DisplayName.Trim(), store.DisplayName);
    Assert.Null(store.Description);
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
    CreateStorePayload payload = new();

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

    this.store.DisplayName = new DisplayNameUnit("Maxi Drummondville");
    this.store.Update(ApplicationContext.ActorId);
    await storeRepository.SaveAsync(this.store);

    ReplaceStorePayload payload = new()
    {
      DisplayName = "Maxi Drummondville",
      Description = "  Supermarché à proximité de la bibliothèque municipale et du terminus d'autobus.  "
    };

    AcceptedCommand command = await storeService.ReplaceAsync(this.store.Id.Value, payload, version);
    Assert.Equal(this.store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.store.Id.Value);
    Assert.NotNull(store);
    Assert.Equal("Maxi Drummondville", store.DisplayName);
    Assert.Equal(payload.Description.Trim(), store.Description);
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
    StoreAggregate iga = new(new DisplayNameUnit("IGA EXTRA")); // TODO(fpion): refactor
    StoreAggregate loblaws = new(new DisplayNameUnit("LOBLAWS")); // TODO(fpion): refactor
    StoreAggregate metro = new(new DisplayNameUnit("METRO INC.")); // TODO(fpion): refactor
    StoreAggregate provigo = new(new DisplayNameUnit("PROVIGO")); // TODO(fpion): refactor
    StoreAggregate superC = new(new DisplayNameUnit("SUPER C")); // TODO(fpion): refactor

    StoreAggregate[] newStores = new[] { iga, loblaws, metro, provigo, superC };
    await storeRepository.SaveAsync(newStores);

    StoreId[] ids = new[] { store.Id, iga.Id, loblaws.Id, metro.Id, superC.Id };
    SearchStoresPayload payload = new()
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
      Sort = new List<StoreSortOption>
      {
        new StoreSortOption(StoreSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    StoreAggregate[] expected = new[] { store, iga, loblaws, metro }.OrderBy(x => x.DisplayName.Value)
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
      Description = new Modification<string>("  Supermarché à proximité de la bibliothèque municipale et du terminus d'autobus.  ")
    };

    AcceptedCommand command = await storeService.UpdateAsync(this.store.Id.Value, payload);
    Assert.Equal(this.store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > this.store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    StoreEntity? store = await FakturContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == this.store.Id.Value);
    Assert.NotNull(store);
    Assert.Equal(payload.Description.Value?.Trim(), store.Description);
  }
}
