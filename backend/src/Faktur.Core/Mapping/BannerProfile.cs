using AutoMapper;
using Faktur.Core.Stores;
using Faktur.Core.Stores.Models;

namespace Faktur.Core.Mapping
{
  internal class BannerProfile : Profile
  {
    public BannerProfile()
    {
      CreateMap<Banner, BannerModel>();
    }
  }
}
