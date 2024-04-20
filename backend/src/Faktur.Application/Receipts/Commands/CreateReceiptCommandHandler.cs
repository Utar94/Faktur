using Faktur.Application.Receipts.Validators;
using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class CreateReceiptCommandHandler : IRequestHandler<CreateReceiptCommand, Receipt>
{
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  public CreateReceiptCommandHandler(IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository,
    IStoreRepository storeRepository, ITaxRepository taxRepository)
  {
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
    _storeRepository = storeRepository;
    _taxRepository = taxRepository;
  }

  public async Task<Receipt> Handle(CreateReceiptCommand command, CancellationToken cancellationToken)
  {
    CreateReceiptPayload payload = command.Payload;
    new CreateReceiptValidator().ValidateAndThrow(payload);

    StoreAggregate store = await _storeRepository.LoadAsync(payload.StoreId, cancellationToken)
      ?? throw new StoreNotFoundException(payload.StoreId, nameof(payload.StoreId));

    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);
    ReceiptAggregate receipt = new(store, payload.IssuedOn, number, taxes, command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
