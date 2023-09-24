using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Faktur.EntityFrameworkCore.Relational;

internal static class Db
{
  internal static class Actors
  {
    public static readonly TableId Table = new(nameof(FakturContext.Actors));
  }

  internal static class Articles
  {
    public static readonly TableId Table = new(nameof(FakturContext.Articles));

    public static readonly ColumnId AggregateId = new(nameof(ArticleEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ArticleEntity.DisplayName), Table);
    public static readonly ColumnId Gtin = new(nameof(ArticleEntity.Gtin), Table);
    public static readonly ColumnId GtinNormalized = new(nameof(ArticleEntity.GtinNormalized), Table);
  }

  internal static class Banners
  {
    public static readonly TableId Table = new(nameof(FakturContext.Banners));

    public static readonly ColumnId AggregateId = new(nameof(ArticleEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ArticleEntity.DisplayName), Table);
  }

  internal static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
  }

  internal static class Stores
  {
    public static readonly TableId Table = new(nameof(FakturContext.Stores));

    public static readonly ColumnId AggregateId = new(nameof(StoreEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(StoreEntity.DisplayName), Table);
  }
}
