using Faktur.Application.Receipts.Validators;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class UpdateReceiptCommandHandler : IRequestHandler<UpdateReceiptCommand, Receipt?>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public UpdateReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<Receipt?> Handle(UpdateReceiptCommand command, CancellationToken cancellationToken)
  {
    UpdateReceiptPayload payload = command.Payload;
    new UpdateReceiptValidator().ValidateAndThrow(payload);

    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.Id, cancellationToken);
    if (receipt == null)
    {
      return null;
    }

    if (payload.IssuedOn.HasValue)
    {
      receipt.IssuedOn = payload.IssuedOn.Value;
    }
    if (payload.Number != null)
    {
      receipt.Number = NumberUnit.TryCreate(payload.Number.Value);
    }

    receipt.Update(command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
