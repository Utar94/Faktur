using AutoMapper;
using Faktur.Core.Receipts;
using Faktur.Core.Receipts.Models;
using Faktur.Core.Stores;

namespace Faktur.Core.Mapping
{
  public class ItemProfile : Profile
  {
    public ItemProfile()
    {
      CreateMap<Item, ItemModel>()
        .ForMember(x => x.Code, x => x.MapFrom(GetCode))
        .ForMember(x => x.Department, x => x.MapFrom(GetDepartment))
        .ForMember(x => x.Flags, x => x.MapFrom(GetFlags))
        .ForMember(x => x.Label, x => x.MapFrom(GetLabel));
    }

    private static string? GetCode(Item item, ItemModel model)
    {
      if (item.Product == null)
      {
        throw new ArgumentException($"The {nameof(item.Product)} is required.", nameof(item));
      }
      else if (item.Product.Article == null)
      {
        throw new ArgumentException($"The {nameof(item.Product.Article)} is required.", nameof(item));
      }

      return item.Product.Sku ?? item.Product.Article.Gtin?.ToString();
    }

    private static string? GetDepartment(Item item, ItemModel model)
    {
      Department? department = item.Product?.Department;
      if (department == null)
      {
        return null;
      }

      return string.Join("-", new[]
      {
        department.Number,
        department.Name
      }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static string? GetFlags(Item item, ItemModel model)
    {
      if (item.Product == null)
      {
        throw new ArgumentException($"The {nameof(item.Product)} is required.", nameof(item));
      }

      return item.Product.Flags;
    }

    private static string GetLabel(Item item, ItemModel model)
    {
      if (item.Product == null)
      {
        throw new ArgumentException($"The {nameof(item.Product)} is required.", nameof(item));
      }
      else if (item.Product.Article == null)
      {
        throw new ArgumentException($"The {nameof(item.Product.Article)} is required.", nameof(item));
      }

      return item.Product.Label ?? item.Product.Article.Name;
    }
  }
}
