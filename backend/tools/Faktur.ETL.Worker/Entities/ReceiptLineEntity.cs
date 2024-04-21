namespace Faktur.ETL.Worker.Entities;

internal class ReceiptLineEntity : AggregateEntity
{
  public ReceiptItemEntity? Item { get; set; }
  public int ItemId { get; set; }

  public string Category { get; set; } = string.Empty;
}
