using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Payloads;
using MediatR;

namespace Faktur.Core.Receipts.Commands
{
  public class UpdateReceipt : IRequest<ReceiptModel>
  {
    public UpdateReceipt(int id, UpdateReceiptPayload payload)
    {
      Id = id;
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public int Id { get; }
    public UpdateReceiptPayload Payload { get; }
  }
}
