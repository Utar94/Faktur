using AutoMapper;
using Faktur.Core.Receipts;
using Faktur.Core.Receipts.Models;

namespace Faktur.Core.Mapping
{
  public class ReceiptProfile : Profile
  {
    public ReceiptProfile()
    {
      CreateMap<Receipt, ReceiptModel>();
    }
  }
}
