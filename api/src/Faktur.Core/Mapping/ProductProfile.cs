using AutoMapper;
using Faktur.Core.Products;
using Faktur.Core.Products.Models;

namespace Faktur.Core.Mapping
{
  internal class ProductProfile : Profile
  {
    public ProductProfile()
    {
      CreateMap<Product, ProductModel>();
    }
  }
}
