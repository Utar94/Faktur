using AutoMapper;
using Faktur.Core.Receipts;
using Faktur.Core.Receipts.Models;

namespace Faktur.Core.Mapping
{
  public class TaxProfile : Profile
  {
    public TaxProfile()
    {
      CreateMap<Tax, TaxModel>();
    }
  }
}
