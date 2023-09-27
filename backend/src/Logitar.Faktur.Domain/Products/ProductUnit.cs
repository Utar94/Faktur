using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Products;

public record ProductUnit
{
  public ArticleId ArticleId { get; }

  public DisplayNameUnit DisplayName { get; }
  public DescriptionUnit? Description { get; }

  public ProductUnit(ArticleId articleId, DisplayNameUnit displayName, DescriptionUnit? description = null)
  {
    ArticleId = articleId;

    DisplayName = displayName;
    Description = description;
  }
}
