using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Payloads;
using MediatR;

namespace Faktur.Core.Receipts.Commands
{
  public class ImportReceipt : IRequest<ReceiptModel>
  {
    public ImportReceipt(ImportReceiptPayload payload)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public ImportReceiptPayload Payload { get; }
  }
}
