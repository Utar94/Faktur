using Faktur.Application.Receipts.Validators;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class CategorizeReceiptCommandHandler : IRequestHandler<CategorizeReceiptCommand, Receipt?>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public CategorizeReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<Receipt?> Handle(CategorizeReceiptCommand command, CancellationToken cancellationToken)
  {
    CategorizeReceiptPayload payload = command.Payload;
    new CategorizeReceiptValidator().ValidateAndThrow(payload);

    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.Id, cancellationToken);
    if (receipt == null)
    {
      return null;
    }

    Dictionary<ushort, CategoryUnit?> itemCategories = new(capacity: payload.ItemCategories.Count);
    foreach (ReceiptItemCategory itemCategory in payload.ItemCategories)
    {
      itemCategories[itemCategory.Number] = CategoryUnit.TryCreate(itemCategory.Category);
    }

    receipt.Categorize(itemCategories, command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
