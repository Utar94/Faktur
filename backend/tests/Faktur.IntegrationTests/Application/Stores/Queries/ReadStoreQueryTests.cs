using Faktur.Contracts.Stores;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadStoreQueryTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  public ReadStoreQueryTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    ReadStoreQuery query = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the store when it is found.")]
  public async Task It_should_return_the_store_when_it_is_found()
  {
    StoreAggregate store = new(new DisplayNameUnit("MAXI"));
    await _storeRepository.SaveAsync(store);

    ReadStoreQuery query = new(store.Id.ToGuid());
    Store? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(store.Id.ToGuid(), result.Id);
  }
}
