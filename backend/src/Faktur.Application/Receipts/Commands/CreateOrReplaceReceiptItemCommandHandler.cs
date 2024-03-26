using Faktur.Application.Receipts.Validators;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class CreateOrReplaceReceiptItemCommandHandler : IRequestHandler<CreateOrReplaceReceiptItemCommand, CreateOrReplaceReceiptItemResult?>
{
  private readonly IReceiptItemQuerier _receiptItemQuerier;
  private readonly IReceiptRepository _receiptRepository;

  public CreateOrReplaceReceiptItemCommandHandler(IReceiptItemQuerier receiptItemQuerier, IReceiptRepository receiptRepository)
  {
    _receiptItemQuerier = receiptItemQuerier;
    _receiptRepository = receiptRepository;
  }

  public async Task<CreateOrReplaceReceiptItemResult?> Handle(CreateOrReplaceReceiptItemCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceReceiptItemPayload payload = command.Payload;
    new CreateOrReplaceReceiptItemValidator().ValidateAndThrow(payload);

    string gtinOrSku = payload.GtinOrSku.Trim();
    GtinUnit? gtin = null;
    SkuUnit? sku = null;
    if (long.TryParse(gtinOrSku, out _))
    {
      gtin = new(gtinOrSku);
    }
    else
    {
      sku = new(gtinOrSku);
    }

    DisplayNameUnit label = new(payload.Label);
    FlagsUnit? flags = FlagsUnit.TryCreate(payload.Flags);
    double quantity = payload.Quantity ?? 1.0;
    decimal price = payload.Price;
    decimal unitPrice = payload.UnitPrice ?? price;

    NumberUnit? departmentNumber = null;
    DepartmentUnit? department = null;
    if (payload.Department != null)
    {
      departmentNumber = new(payload.Department.Number);
      department = new(new DisplayNameUnit(payload.Department.DisplayName), DescriptionUnit.TryCreate(payload.Department.Description));
    }

    ReceiptAggregate? receipt = await _receiptRepository.LoadAsync(command.ReceiptId, cancellationToken);
    if (receipt == null)
    {
      return null;
    }
    ReceiptItemUnit? reference = null;
    if (command.Version.HasValue)
    {
      ReceiptAggregate? receiptReference = await _receiptRepository.LoadAsync(command.ReceiptId, command.Version.Value, cancellationToken);
      if (receiptReference != null)
      {
        reference = receiptReference.TryFindItem(command.ItemNumber);
      }
    }

    ReceiptItemUnit? item = receipt.TryFindItem(command.ItemNumber);
    bool isCreated = item == null;
    if (item != null && reference != null)
    {
      if (label == reference.Label)
      {
        label = item.Label;
      }
      if (flags == reference.Flags)
      {
        flags = item.Flags;
      }
      if (quantity == reference.Quantity)
      {
        quantity = item.Quantity;
      }
      if (unitPrice == reference.UnitPrice)
      {
        unitPrice = item.UnitPrice;
      }
      if (price == reference.Price)
      {
        price = item.Price;
      }

      if (departmentNumber == reference.DepartmentNumber && department == reference.Department)
      {
        departmentNumber = item.DepartmentNumber;
        department = item.Department;
      }
    }

    item = new(gtin, sku, label, flags, quantity, unitPrice, price, departmentNumber, department);
    receipt.SetItem(command.ItemNumber, item, command.ActorId);

    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    ReceiptItem result = await _receiptItemQuerier.ReadAsync(receipt, command.ItemNumber, cancellationToken);
    return new CreateOrReplaceReceiptItemResult(isCreated, result);
  }
}
