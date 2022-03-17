using AutoMapper;
using Faktur.Core.Stores;
using Faktur.Core.Stores.Models;

namespace Faktur.Core.Mapping
{
  internal class StoreProfile : Profile
  {
    public StoreProfile()
    {
      CreateMap<Store, StoreModel>();
    }
  }
}
