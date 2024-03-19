using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Receipts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteReceiptCommandTests : IntegrationTests
{
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public DeleteReceiptCommandTests() : base()
  {
    _receiptRepository = ServiceProvider.GetRequiredService<IReceiptRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Receipts.Table, FakturDb.Stores.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing receipt.")]
  public async Task It_should_delete_an_existing_receipt()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    await _storeRepository.SaveAsync(store);

    ReceiptAggregate receipt = new(store, number: new NumberUnit("117011"));
    await _receiptRepository.SaveAsync(receipt);

    DeleteReceiptCommand command = new(receipt.Id.ToGuid());
    Receipt? result = await Mediator.Send(command);
    Assert.NotNull(result);
    Assert.Equal(receipt.Id.ToGuid(), result.Id);

    Assert.Empty(await FakturContext.Receipts.ToArrayAsync());
  }

  [Fact(DisplayName = "It should return null when the receipt cannot be found.")]
  public async Task It_should_return_null_when_the_receipt_cannot_be_found()
  {
    DeleteReceiptCommand command = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(command));
  }
}
