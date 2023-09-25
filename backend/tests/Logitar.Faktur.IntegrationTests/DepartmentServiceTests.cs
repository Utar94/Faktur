using Logitar.EventSourcing;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class DepartmentServiceTests : IntegrationTests
{
  private readonly IBannerRepository bannerRepository;
  private readonly IDepartmentService departmentService;
  private readonly IStoreRepository storeRepository;

  private readonly BannerAggregate banner;
  private readonly StoreAggregate store;

  public DepartmentServiceTests()
  {
    bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    departmentService = ServiceProvider.GetRequiredService<IDepartmentService>();
    storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    BannerId bannerId = new(new AggregateId("MAXI"));
    banner = new(new DisplayNameUnit("Maxi"), ApplicationContext.ActorId, bannerId);

    StoreNumberUnit number = new("08772");
    StoreId storeId = new(banner, number);
    store = new StoreAggregate(new DisplayNameUnit("Maxi Drummondville"), ApplicationContext.ActorId, storeId)
    {
      Address = new AddressUnit("1870 boul Saint-Joseph", "Drummondville", "CA", "QC", "J2B 1R2"),
      Phone = new PhoneUnit("+18194721197", "CA")
    };
    store.Update(ApplicationContext.ActorId);

    DepartmentUnit department = new(new DepartmentNumberUnit("21"), new DisplayNameUnit("EPICERIE"));
    store.SetDepartment(department, ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await bannerRepository.SaveAsync(banner);
    await storeRepository.SaveAsync(store);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct department by ID.")]
  public async Task ReadAsync_it_should_read_the_correct_department_by_id()
  {
    DepartmentUnit existing = store.Departments.Values.Single();

    Department? department = await departmentService.ReadAsync(store.Id.Value, store.Departments.Keys.Single());
    Assert.NotNull(department);

    Assert.Equal(store.Departments.Keys.Single(), department.Number);
    Assert.Equal(existing.DisplayName.Value, department.DisplayName);
    Assert.Equal(existing.Description?.Value, department.Description);

    Assert.NotNull(department.Store);
    Assert.Equal(store.Id.Value, department.Store.Id);

    Assert.Equal(1, department.Version);
    Assert.Equal(ApplicationContext.Actor.Id, department.CreatedBy.Id);
    Assert.Equal(ApplicationContext.Actor.Id, department.UpdatedBy.Id);
    AssertAreNear(store.UpdatedOn, department.CreatedOn);
    Assert.Equal(department.CreatedOn, department.UpdatedOn);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when no department are found.")]
  public async Task ReadAsync_it_should_return_null_when_no_department_are_found()
  {
    Assert.Null(await departmentService.ReadAsync(store.Id.Value, number: "22"));
  }

  [Fact(DisplayName = "RemoveAsync: it should remove the found department.")]
  public async Task RemoveAsync_it_should_remove_the_found_department()
  {
    string number = store.Departments.Keys.Single();

    AcceptedCommand command = await departmentService.RemoveAsync(store.Id.Value, number);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);
  }

  [Fact(DisplayName = "RemoveAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task RemoveAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await departmentService.RemoveAsync(storeId.Value, number: "22")
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "RemoveAsync: it should throw DepartmentNotFoundException when the department could not be found.")]
  public async Task RemoveAsync_it_should_throw_DepartmentNotFoundException_when_the_department_could_not_be_found()
  {
    string number = "22";
    var exception = await Assert.ThrowsAsync<DepartmentNotFoundException>(
      async () => await departmentService.RemoveAsync(store.Id.Value, number)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(number, exception.Number.Value);
    Assert.Equal("Number", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should save a new department.")]
  public async Task SaveAsync_it_should_save_a_new_department()
  {
    string number = "22";
    SaveDepartmentPayload payload = new()
    {
      DisplayName = "  PRODUITS LAITIERS  ",
      Description = "    "
    };

    AcceptedCommand command = await departmentService.SaveAsync(store.Id.Value, number, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    DepartmentEntity? department = await FakturContext.Departments.AsNoTracking()
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Number == number);
    Assert.NotNull(department);
    Assert.NotNull(department.Store);
    Assert.Equal(store.Id.Value, department.Store.AggregateId);

    Assert.Equal(number, department.Number);
    Assert.Equal(int.Parse(number), department.NumberNormalized);
    Assert.Equal(payload.DisplayName.Trim(), department.DisplayName);
    Assert.Null(department.Description);

    Assert.Equal(1, department.Version);
    Assert.Equal(ApplicationContext.Actor.Id, department.CreatedBy);
    AssertIsNear(AsUniversalTime(department.CreatedOn));
    Assert.Equal(ApplicationContext.Actor.Id, department.UpdatedBy);
    Assert.Equal(department.CreatedOn, department.UpdatedOn);
  }

  [Fact(DisplayName = "SaveAsync: it should save an existing department.")]
  public async Task SaveAsync_it_should_save_an_existing_department()
  {
    string number = store.Departments.Keys.Single();
    SaveDepartmentPayload payload = new()
    {
      DisplayName = "  ÉPICERIE  ",
      Description = "  Département comprenant des produits généraux.  "
    };

    AcceptedCommand command = await departmentService.SaveAsync(store.Id.Value, number, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    DepartmentEntity? department = await FakturContext.Departments.AsNoTracking()
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.NumberNormalized == int.Parse(number));
    Assert.NotNull(department);
    Assert.NotNull(department.Store);
    Assert.Equal(store.Id.Value, department.Store.AggregateId);

    Assert.Equal(number, department.Number);
    Assert.Equal(int.Parse(number), department.NumberNormalized);
    Assert.Equal(payload.DisplayName.Trim(), department.DisplayName);
    Assert.Equal(payload.Description.Trim(), department.Description);

    Assert.Equal(2, department.Version);
    Assert.Equal(ApplicationContext.Actor.Id, department.CreatedBy);
    Assert.Equal(ApplicationContext.Actor.Id, department.UpdatedBy);
    AssertIsNear(AsUniversalTime(department.UpdatedOn));
    Assert.True(department.UpdatedOn > department.CreatedOn);
  }

  [Fact(DisplayName = "SaveAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task SaveAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    SaveDepartmentPayload payload = new()
    {
      DisplayName = "PRODUITS LAITIERS"
    };

    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await departmentService.SaveAsync(storeId.Value, number: "22", payload)
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task SaveAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveDepartmentPayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(
      async () => await departmentService.SaveAsync(store.Id.Value, number: "22", payload)
    );
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchDepartmentsPayload payload = new();
    payload.Id.Terms.Add(new SearchTerm(Guid.Empty.ToString()));

    SearchResults<Department> departments = await departmentService.SearchAsync(payload);

    Assert.Empty(departments.Results);
    Assert.Equal(0, departments.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    DepartmentUnit department = store.Departments.Values.Single();

    StoreNumberUnit storeNumber = new("08984");
    StoreAggregate otherStore = new(new DisplayNameUnit("Maxi Drummondville St-Joseph"), ApplicationContext.ActorId, new StoreId(banner, storeNumber));
    DepartmentUnit otherDepartment = new(new DepartmentNumberUnit("21"), new DisplayNameUnit("EPICERIE"));
    otherStore.SetDepartment(otherDepartment, ApplicationContext.ActorId);

    DepartmentUnit boulangerie = new(new DepartmentNumberUnit("33"), new DisplayNameUnit("BOULANGERIE"));
    store.SetDepartment(boulangerie, ApplicationContext.ActorId);

    DepartmentUnit boulangerieCommerciale = new(new DepartmentNumberUnit("34"), new DisplayNameUnit("BOULANGERIE COMMERCIALE"));
    store.SetDepartment(boulangerieCommerciale, ApplicationContext.ActorId);

    DepartmentUnit charcuterie = new(new DepartmentNumberUnit("35"), new DisplayNameUnit("CHARCUTERIE"));
    store.SetDepartment(charcuterie, ApplicationContext.ActorId);

    DepartmentUnit produitsLaitiers = new(new DepartmentNumberUnit("22"), new DisplayNameUnit("PRODUITS LAITIERS"));
    store.SetDepartment(produitsLaitiers, ApplicationContext.ActorId);

    DepartmentUnit viande = new(new DepartmentNumberUnit("31"), new DisplayNameUnit("VIANDE"));
    store.SetDepartment(viande, ApplicationContext.ActorId);

    await storeRepository.SaveAsync(new[] { store, otherStore });

    DepartmentNumberUnit[] departmentNumbers = new[]
    {
      department.Number, otherDepartment.Number, boulangerie.Number, boulangerieCommerciale.Number, charcuterie.Number, viande.Number
    };
    SearchDepartmentsPayload payload = new()
    {
      Id = new TextSearch
      {
        Terms = departmentNumbers.Select(number => new SearchTerm(number.Value)).ToList(),
        Operator = SearchOperator.Or
      },
      Search = new TextSearch
      {
        Terms = new List<SearchTerm>
        {
          new("21"),
          new("_5"),
          new("b__l_n%"),
          new("%lait%")
        },
        Operator = SearchOperator.Or
      },
      StoreId = store.Id.Value,
      Sort = new List<DepartmentSortOption>
      {
        new(DepartmentSort.Number, isDescending: true)
      },
      Skip = 1,
      Limit = 2
    };

    DepartmentUnit[] expected = new[] { department, boulangerie, boulangerieCommerciale, charcuterie }
      .OrderByDescending(x => x.Number.NormalizedValue).Skip(payload.Skip).Take(payload.Limit).ToArray();

    SearchResults<Department> departments = await departmentService.SearchAsync(payload);
    Assert.Equal(4, departments.Total);

    Assert.Equal(expected.Length, departments.Results.Count);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i].Number.Value, departments.Results[i].Number);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    UpdateDepartmentPayload payload = new();

    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await departmentService.UpdateAsync(storeId.Value, number: "22", payload)
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw DepartmentNotFoundException when the department could not be found.")]
  public async Task UpdateAsync_it_should_throw_DepartmentNotFoundException_when_the_department_could_not_be_found()
  {
    UpdateDepartmentPayload payload = new();

    string number = "22";
    var exception = await Assert.ThrowsAsync<DepartmentNotFoundException>(
      async () => await departmentService.UpdateAsync(store.Id.Value, number, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(number, exception.Number.Value);
    Assert.Equal("Number", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ValidationException when the department could not be found.")]
  public async Task UpdateAsync_it_should_throw_ValidationException_when_the_department_could_not_be_found()
  {
    string number = store.Departments.Keys.Single();
    UpdateDepartmentPayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(
      async () => await departmentService.UpdateAsync(store.Id.Value, number, payload)
    );
  }

  [Fact(DisplayName = "UpdateAsync: it should update an existing department.")]
  public async Task UpdateAsync_it_should_update_an_existing_department()
  {
    string number = store.Departments.Keys.Single();
    UpdateDepartmentPayload payload = new()
    {
      Description = new Modification<string>("  Département comprenant des produits généraux.  ")
    };

    AcceptedCommand command = await departmentService.UpdateAsync(store.Id.Value, number, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    DepartmentEntity? department = await FakturContext.Departments.AsNoTracking()
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.NumberNormalized == int.Parse(number));
    Assert.NotNull(department);
    Assert.NotNull(department.Store);
    Assert.Equal(store.Id.Value, department.Store.AggregateId);

    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.Trim(), department.Description);

    Assert.Equal(2, department.Version);
    Assert.Equal(ApplicationContext.Actor.Id, department.CreatedBy);
    Assert.Equal(ApplicationContext.Actor.Id, department.UpdatedBy);
    AssertIsNear(AsUniversalTime(department.UpdatedOn));
    Assert.True(department.UpdatedOn > department.CreatedOn);
  }
}
