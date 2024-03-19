using Faktur.Domain.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class DeleteStoreReceiptsCommandHandler : INotificationHandler<DeleteStoreReceiptsCommand>
{
  private readonly IReceiptRepository _receiptRepository;

  public DeleteStoreReceiptsCommandHandler(IReceiptRepository receiptRepository)
  {
    _receiptRepository = receiptRepository;
  }

  public async Task Handle(DeleteStoreReceiptsCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<ReceiptAggregate> receipts = await _receiptRepository.LoadAsync(command.Store, cancellationToken);
    foreach (ReceiptAggregate receipt in receipts)
    {
      receipt.Delete(command.ActorId);
    }
    await _receiptRepository.SaveAsync(receipts, cancellationToken);
  }
}
