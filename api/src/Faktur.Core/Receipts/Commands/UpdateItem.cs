using Faktur.Core.Receipts.Models;
using Faktur.Core.Receipts.Payloads;
using MediatR;

namespace Faktur.Core.Receipts.Commands
{
  public class UpdateItem : IRequest<ReceiptModel>
  {
    public UpdateItem(int id, UpdateItemPayload payload)
    {
      Id = id;
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public int Id { get; }
    public UpdateItemPayload Payload { get; }
  }
}
