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

    IEnumerable<ReceiptItemUnit> items = [];
    if (!string.IsNullOrWhiteSpace(payload.Lines))
    {
      LocaleUnit? locale = LocaleUnit.TryCreate(payload.Locale);
      items = await _receiptParser.ExecuteAsync(payload.Lines, locale, cancellationToken);

      foreach (ReceiptItemUnit item in items)
      {
        if (item.DepartmentNumber != null && item.Department != null && !store.HasDepartment(item.DepartmentNumber))
        {
          store.SetDepartment(item.DepartmentNumber, item.Department, command.ActorId);
        }
      }

      // TODO(fpion): create missing articles
      // TODO(fpion): create missing products
    }

    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    ReceiptAggregate receipt = ReceiptAggregate.Import(store, payload.IssuedOn, number, items, command.ActorId);

    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);
    receipt.Calculate(taxes, command.ActorId);

    await _storeRepository.SaveAsync(store, cancellationToken);
    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }
}
