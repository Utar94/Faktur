using Faktur.Core.Receipts.Models;
using MediatR;

namespace Faktur.Core.Receipts.Queries
{
  public class GetReceipt : IRequest<ReceiptModel>
  {
    public GetReceipt(int id)
    {
      Id = id;
    }

    public int Id { get; }
  }
}
