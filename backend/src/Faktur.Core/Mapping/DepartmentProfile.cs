using AutoMapper;
using Faktur.Core.Stores;
using Faktur.Core.Stores.Models;

namespace Faktur.Core.Mapping
{
  internal class DepartmentProfile : Profile
  {
    public DepartmentProfile()
    {
      CreateMap<Department, DepartmentModel>();
    }
  }
}
