using Faktur.Contracts.Stores;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Stores.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteStoreCommandTests : IntegrationTests
{
  private readonly IStoreRepository _storeRepository;

  public DeleteStoreCommandTests() : base()
  {
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Stores.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing store.")]
  public async Task It_should_delete_an_existing_store()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    await _storeRepository.SaveAsync(store);

    DeleteStoreCommand command = new(store.Id.ToGuid());
    Store? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(store.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Stores.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the store cannot be found.")]
  public async Task It_should_return_null_when_the_store_cannot_be_found()
  {
    DeleteStoreCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }

  // TODO(fpion): Delete Store Receipts
}
