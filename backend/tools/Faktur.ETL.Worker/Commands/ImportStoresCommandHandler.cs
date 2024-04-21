using Faktur.Contracts.Departments;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportStoresCommandHandler : IRequestHandler<ImportStoresCommand, int>
{
  private readonly IStoreRepository _storeRepository;

  public ImportStoresCommandHandler(IStoreRepository storeRepository)
  {
    _storeRepository = storeRepository;
  }

  public async Task<int> Handle(ImportStoresCommand command, CancellationToken cancellationToken)
  {
    Dictionary<Guid, StoreAggregate> stores = (await _storeRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Id.ToGuid(), x => x);
    int count = 0;
    foreach (Store store in command.Stores)
    {
      StoreId id = new(store.Id);

      DisplayNameUnit displayName = new(store.DisplayName);
      if (stores.TryGetValue(store.Id, out StoreAggregate? existingStore))
      {
        existingStore.DisplayName = displayName;
      }
      else
      {
        ActorId createdBy = new(store.CreatedBy.Id);
        existingStore = new(displayName, createdBy, id);
        stores[store.Id] = existingStore;
      }

      existingStore.BannerId = store.Banner == null ? null : new BannerId(store.Banner.Id);
      existingStore.Number = NumberUnit.TryCreate(store.Number);
      existingStore.Description = DescriptionUnit.TryCreate(store.Description);

      existingStore.Address = store.Address == null ? null : new AddressUnit(store.Address.Street, store.Address.Locality, store.Address.Country, store.Address.Region, store.Address.PostalCode, isVerified: false);
      existingStore.Phone = store.Phone == null ? null : new PhoneUnit(store.Phone.Number, store.Phone.CountryCode, store.Phone.Extension, isVerified: false);

      foreach (Department storeDepartment in store.Departments)
      {
        NumberUnit number = new(storeDepartment.Number);
        DepartmentUnit department = new(new DisplayNameUnit(storeDepartment.DisplayName), DescriptionUnit.TryCreate(storeDepartment.Description));
        ActorId actorId = new(storeDepartment.UpdatedBy.Id);
        existingStore.SetDepartment(number, department, actorId);
      }

      ActorId updatedBy = new(store.UpdatedBy.Id);
      existingStore.Update(updatedBy);

      if (existingStore.HasChanges)
      {
        existingStore.SetDates(store);
        count++;
      }
    }

    await _storeRepository.SaveAsync(stores.Values, cancellationToken);

    return count;
  }
}
