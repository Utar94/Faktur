using Logitar.Data;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Faktur.EntityFrameworkCore.Relational;

internal static class Db
{
  internal static class Articles
  {
    public static readonly TableId Table = new(nameof(FakturContext.Articles));

    public static readonly ColumnId AggregateId = new(nameof(ArticleEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ArticleEntity.DisplayName), Table);
    public static readonly ColumnId Gtin = new(nameof(ArticleEntity.Gtin), Table);
  }
}
