using Faktur.Core.Receipts.Models;
using MediatR;

namespace Faktur.Core.Receipts.Commands
{
  public class DeleteReceipt : IRequest<ReceiptModel>
  {
    public DeleteReceipt(int id)
    {
      Id = id;
    }

    public int Id { get; }
  }
}
