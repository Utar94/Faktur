using Faktur.Application.Receipts.Validators;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class UpdateReceiptItemCommandHandler : IRequestHandler<UpdateReceiptItemCommand, ReceiptItem?>
{
  private readonly IReceiptItemQuerier _receiptItemQuerier;
  private readonly IReceiptRepository _receiptRepository;
  private readonly ITaxRepository _taxRepository;

  public UpdateReceiptItemCommandHandler(IReceiptItemQuerier receiptItemQuerier, IReceiptRepository receiptRepository, ITaxRepository taxRepository)
  {
    _receiptItemQuerier = receiptItemQuerier;
    _receiptRepository = receiptRepository;
    _taxRepository = taxRepository;
  }

  public async Task<ReceiptItem?> Handle(UpdateReceiptItemCommand command, CancellationToken cancellationToken)
  {
    UpdateReceiptItemPayload payload = command.Payload;
    new UpdateReceiptItemValidator().ValidateAndThrow(payload);

    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.ReceiptId, cancellationToken);
    ReceiptItemUnit? item = receipt?.TryFindItem(command.ItemNumber);
    if (receipt == null || item == null)
    {
      return null;
    }

    GtinUnit? gtin = item.Gtin;
    SkuUnit? sku = item.Sku;
    if (!string.IsNullOrWhiteSpace(payload.GtinOrSku))
    {
      string gtinOrSku = payload.GtinOrSku.Trim();
      if (long.TryParse(gtinOrSku, out _))
      {
        gtin = new(gtinOrSku);
        sku = null;
      }
      else
      {
        gtin = null;
        sku = new(gtinOrSku);
      }
    }

    DisplayNameUnit label = string.IsNullOrWhiteSpace(payload.Label) ? item.Label : new(payload.Label);
    FlagsUnit? flags = payload.Flags == null ? item.Flags : FlagsUnit.TryCreate(payload.Flags.Value);
    double quantity = payload.Quantity ?? item.Quantity;
    decimal unitPrice = payload.UnitPrice ?? item.UnitPrice;
    decimal price = payload.Price ?? item.Price;

    NumberUnit? departmentNumber = item.DepartmentNumber;
    DepartmentUnit? department = item.Department;
    if (payload.Department != null)
    {
      if (payload.Department.Value == null)
      {
        departmentNumber = null;
        department = null;
      }
      else
      {
        departmentNumber = new(payload.Department.Value.Number);
        department = new(new DisplayNameUnit(payload.Department.Value.DisplayName), DescriptionUnit.TryCreate(payload.Department.Value.Description));
      }
    }

    item = new(gtin, sku, label, flags, quantity, unitPrice, price, departmentNumber, department);
    receipt.SetItem(command.ItemNumber, item, command.ActorId);

    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);
    receipt.Calculate(taxes);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptItemQuerier.ReadAsync(receipt, command.ItemNumber, cancellationToken);
  }
}
