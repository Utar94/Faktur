using Faktur.Application.Receipts.Validators;
using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class CreateReceiptCommandHandler : IRequestHandler<CreateReceiptCommand, Receipt>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;

  public CreateReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository, IStoreRepository storeRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
    _storeRepository = storeRepository;
  }

  public async Task<Receipt> Handle(CreateReceiptCommand command, CancellationToken cancellationToken)
  {
    CreateReceiptPayload payload = command.Payload;
    new CreateReceiptValidator().ValidateAndThrow(payload);

    StoreAggregate store = await _storeRepository.LoadAsync(command.StoreId, cancellationToken)
      ?? throw new StoreNotFoundException(command.StoreId, nameof(command.StoreId));

    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    ReceiptAggregate receipt = new(store, payload.IssuedOn, number, command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
