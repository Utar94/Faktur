using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class RemoveReceiptItemCommandHandler : IRequestHandler<RemoveReceiptItemCommand, ReceiptItem?>
{
  private readonly IReceiptItemQuerier _receiptItemQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public RemoveReceiptItemCommandHandler(IReceiptItemQuerier receiptItemQuerier, IReceiptRepository receiptRepository)
  {
    _receiptItemQuerier = receiptItemQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<ReceiptItem?> Handle(RemoveReceiptItemCommand command, CancellationToken cancellationToken)
  {
    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.ReceiptId, cancellationToken);
    if (receipt == null || !receipt.HasItem(command.ItemNumber))
    {
      return null;
    }
    ReceiptItem result = await _receiptItemQuerier.ReadAsync(receipt, command.ItemNumber, cancellationToken);

    receipt.RemoveItem(command.ItemNumber, command.ActorId);
    receipt.Calculate(command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return result;
  }
}
