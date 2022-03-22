using Faktur.Core.Models;
using Faktur.Core.Receipts.Models;
using MediatR;

namespace Faktur.Core.Receipts.Queries
{
  public class GetReceipts : IRequest<ListModel<ReceiptModel>>
  {
    public bool? Deleted { get; set; }
    public string? Search { get; set; }
    public int? StoreId { get; set; }

    public ReceiptSort? Sort { get; set; }
    public bool Desc { get; set; }

    public int? Index { get; set; }
    public int? Count { get; set; }
  }
}
