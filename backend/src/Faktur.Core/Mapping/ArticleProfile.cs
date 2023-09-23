using AutoMapper;
using Faktur.Core.Articles;
using Faktur.Core.Articles.Models;

namespace Faktur.Core.Mapping
{
  internal class ArticleProfile : Profile
  {
    public ArticleProfile()
    {
      CreateMap<Article, ArticleModel>();
    }
  }
}
