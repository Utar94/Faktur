using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Faktur.EntityFrameworkCore.Relational;

internal static class FakturDb
{
  public static class Actors
  {
    public static readonly TableId Table = new(nameof(FakturContext.Actors));

    public static readonly ColumnId ActorId = new(nameof(ActorEntity.ActorId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ActorEntity.DisplayName), Table);
    public static readonly ColumnId EmailAddress = new(nameof(ActorEntity.EmailAddress), Table);
    public static readonly ColumnId Id = new(nameof(ActorEntity.Id), Table);
    public static readonly ColumnId IsDeleted = new(nameof(ActorEntity.IsDeleted), Table);
    public static readonly ColumnId PictureUrl = new(nameof(ActorEntity.PictureUrl), Table);
    public static readonly ColumnId Type = new(nameof(ActorEntity.Type), Table);
  }

  public static class Banners
  {
    public static readonly TableId Table = new(nameof(FakturContext.Banners));

    public static readonly ColumnId AggregateId = new(nameof(BannerEntity.AggregateId), Table);
    public static readonly ColumnId BannerId = new(nameof(BannerEntity.BannerId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(BannerEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(BannerEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(BannerEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(BannerEntity.DisplayName), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(BannerEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(BannerEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(BannerEntity.Version), Table);
  }

  public static class Stores
  {
    public static readonly TableId Table = new(nameof(FakturContext.Stores));

    public static readonly ColumnId AggregateId = new(nameof(StoreEntity.AggregateId), Table);
    public static readonly ColumnId BannerId = new(nameof(StoreEntity.BannerId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(StoreEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(StoreEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(StoreEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(StoreEntity.DisplayName), Table);
    public static readonly ColumnId Number = new(nameof(StoreEntity.Number), Table);
    public static readonly ColumnId StoreId = new(nameof(StoreEntity.StoreId), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(StoreEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(StoreEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(StoreEntity.Version), Table);
  }
}
