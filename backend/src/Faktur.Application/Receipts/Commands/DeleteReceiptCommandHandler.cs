using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class DeleteReceiptCommandHandler : IRequestHandler<DeleteReceiptCommand, Receipt?>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public DeleteReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<Receipt?> Handle(DeleteReceiptCommand command, CancellationToken cancellationToken)
  {
    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.Id, cancellationToken);
    if (receipt == null)
    {
      return null;
    }
    Receipt result = await _receiptQuerier.ReadAsync(receipt, cancellationToken);

    receipt.Delete(command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return result;
  }
}
