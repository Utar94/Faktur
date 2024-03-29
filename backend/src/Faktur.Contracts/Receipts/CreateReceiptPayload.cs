﻿namespace Faktur.Contracts.Receipts;

public record CreateReceiptPayload
{
  public Guid StoreId { get; set; }
  public DateTime? IssuedOn { get; set; }
  public string? Number { get; set; }
}
