namespace Faktur.Contracts.Receipts;

public record CreateOrReplaceReceiptItemResult(bool IsCreated, ReceiptItem Item);
