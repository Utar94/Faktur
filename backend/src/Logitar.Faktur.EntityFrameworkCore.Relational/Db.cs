﻿using Logitar.Data;
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

    public static readonly ColumnId AggregateId = new(nameof(BannerEntity.AggregateId), Table);
    public static readonly ColumnId BannerId = new(nameof(BannerEntity.BannerId), Table);
    public static readonly ColumnId DisplayName = new(nameof(BannerEntity.DisplayName), Table);
  }

  internal static class Departments
  {
    public static readonly TableId Table = new(nameof(FakturContext.Departments));

    public static readonly ColumnId DisplayName = new(nameof(DepartmentEntity.DisplayName), Table);
    public static readonly ColumnId Number = new(nameof(DepartmentEntity.Number), Table);
    public static readonly ColumnId StoreId = new(nameof(DepartmentEntity.StoreId), Table);
  }

  internal static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
  }

  internal static class Products
  {
    public static readonly TableId Table = new(nameof(FakturContext.Products));
  }

  internal static class Stores
  {
    public static readonly TableId Table = new(nameof(FakturContext.Stores));

    public static readonly ColumnId AddressFormatted = new(nameof(StoreEntity.AddressFormatted), Table);
    public static readonly ColumnId AggregateId = new(nameof(StoreEntity.AggregateId), Table);
    public static readonly ColumnId BannerId = new(nameof(StoreEntity.BannerId), Table);
    public static readonly ColumnId DisplayName = new(nameof(StoreEntity.DisplayName), Table);
    public static readonly ColumnId Number = new(nameof(StoreEntity.Number), Table);
    public static readonly ColumnId PhoneE164Formatted = new(nameof(StoreEntity.PhoneE164Formatted), Table);
    public static readonly ColumnId StoreId = new(nameof(StoreEntity.StoreId), Table);
  }
}
