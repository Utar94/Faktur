using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Payloads;
using MediatR;

namespace Faktur.Core.Receipts.Commands
{
  public class ProcessReceipt : IRequest<ReceiptModel>
  {
    public ProcessReceipt(int id, ProcessReceiptPayload payload)
    {
      Id = id;
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public int Id { get; }
    public ProcessReceiptPayload Payload { get; }
  }
}
