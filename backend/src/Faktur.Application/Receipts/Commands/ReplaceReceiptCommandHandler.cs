using Faktur.Application.Receipts.Validators;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class ReplaceReceiptCommandHandler : IRequestHandler<ReplaceReceiptCommand, Receipt?>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public ReplaceReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<Receipt?> Handle(ReplaceReceiptCommand command, CancellationToken cancellationToken)
  {
    ReplaceReceiptPayload payload = command.Payload;
    new ReplaceReceiptValidator().ValidateAndThrow(payload);

    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.Id, cancellationToken);
    if (receipt == null)
    {
      return null;
    }
    ReceiptAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _receiptRepository.LoadAsync(command.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || payload.IssuedOn != reference.IssuedOn)
    {
      receipt.IssuedOn = payload.IssuedOn;
    }
    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    if (reference == null || number != reference.Number)
    {
      receipt.Number = number;
    }

    receipt.Update(command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
