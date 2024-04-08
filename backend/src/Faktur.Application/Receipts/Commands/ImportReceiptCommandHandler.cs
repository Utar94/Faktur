using Faktur.Application.Receipts.Parsing;
using Faktur.Application.Receipts.Validators;
using Faktur.Application.Stores;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class ImportReceiptCommandHandler : IRequestHandler<ImportReceiptCommand, Receipt>
{
  private readonly IReceiptParser _receiptParser;
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  public ImportReceiptCommandHandler(IReceiptParser receiptParser, IReceiptQuerier receiptQuerier,
    IReceiptRepository receiptRepository, IStoreRepository storeRepository, ITaxRepository taxRepository)
  {
    _receiptParser = receiptParser;
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
    _storeRepository = storeRepository;
    _taxRepository = taxRepository;
  }

  public async Task<Receipt> Handle(ImportReceiptCommand command, CancellationToken cancellationToken)
  {
    ImportReceiptPayload payload = command.Payload;
    new ImportReceiptValidator().ValidateAndThrow(payload);

    StoreAggregate store = await _storeRepository.LoadAsync(payload.StoreId, cancellationToken)
      ?? throw new StoreNotFoundException(payload.StoreId, nameof(payload.StoreId));

    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    ReceiptAggregate receipt = new(store, payload.IssuedOn, number, command.ActorId);

    if (!string.IsNullOrWhiteSpace(payload.Lines))
    {
      LocaleUnit? locale = LocaleUnit.TryCreate(payload.Locale);
      ReceiptItemUnit[] items = (await _receiptParser.ExecuteAsync(payload.Lines, locale, cancellationToken)).ToArray();
      for (ushort itemNumber = 0; itemNumber < items.Length; itemNumber++)
      {
        receipt.SetItem(itemNumber, items[itemNumber], command.ActorId); // TODO(fpion): optimize (2+N events)
      }
    }

    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);
    receipt.Calculate(taxes, command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
