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

  public static class Articles
  {
    public static readonly TableId Table = new(nameof(FakturContext.Articles));

    public static readonly ColumnId AggregateId = new(nameof(ArticleEntity.AggregateId), Table);
    public static readonly ColumnId ArticleId = new(nameof(ArticleEntity.ArticleId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ArticleEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ArticleEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ArticleEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ArticleEntity.DisplayName), Table);
    public static readonly ColumnId Gtin = new(nameof(ArticleEntity.Gtin), Table);
    public static readonly ColumnId GtinNormalized = new(nameof(ArticleEntity.GtinNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ArticleEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ArticleEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ArticleEntity.Version), Table);
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

  public static class Departments
  {
    public static readonly TableId Table = new(nameof(FakturContext.Departments));

    public static readonly ColumnId CreatedBy = new(nameof(DepartmentEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(DepartmentEntity.CreatedOn), Table);
    public static readonly ColumnId DepartmentId = new(nameof(DepartmentEntity.DepartmentId), Table);
    public static readonly ColumnId Description = new(nameof(DepartmentEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(DepartmentEntity.DisplayName), Table);
    public static readonly ColumnId Number = new(nameof(DepartmentEntity.Number), Table);
    public static readonly ColumnId NumberNormalized = new(nameof(DepartmentEntity.NumberNormalized), Table);
    public static readonly ColumnId StoreId = new(nameof(DepartmentEntity.StoreId), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(DepartmentEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(DepartmentEntity.UpdatedOn), Table);
  }

  public static class Products
  {
    public static readonly TableId Table = new(nameof(FakturContext.Products));

    public static readonly ColumnId AggregateId = new(nameof(ProductEntity.AggregateId), Table);
    public static readonly ColumnId ArticleId = new(nameof(ProductEntity.ArticleId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ProductEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ProductEntity.CreatedOn), Table);
    public static readonly ColumnId DepartmentId = new(nameof(ProductEntity.DepartmentId), Table);
    public static readonly ColumnId Description = new(nameof(ProductEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ProductEntity.DisplayName), Table);
    public static readonly ColumnId Flags = new(nameof(ProductEntity.Flags), Table);
    public static readonly ColumnId ProductId = new(nameof(ProductEntity.ProductId), Table);
    public static readonly ColumnId Sku = new(nameof(ProductEntity.Sku), Table);
    public static readonly ColumnId SkuNormalized = new(nameof(ProductEntity.SkuNormalized), Table);
    public static readonly ColumnId StoreId = new(nameof(ProductEntity.StoreId), Table);
    public static readonly ColumnId UnitPrice = new(nameof(ProductEntity.UnitPrice), Table);
    public static readonly ColumnId UnitType = new(nameof(ProductEntity.UnitType), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ProductEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ProductEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ProductEntity.Version), Table);
  }

  public static class ReceiptItems
  {
    public static readonly TableId Table = new(nameof(FakturContext.ReceiptItems));

    public static readonly ColumnId DepartmentName = new(nameof(ReceiptItemEntity.DepartmentName), Table);
    public static readonly ColumnId DepartmentNumber = new(nameof(ReceiptItemEntity.DepartmentNumber), Table);
    public static readonly ColumnId Flags = new(nameof(ReceiptItemEntity.Flags), Table);
    public static readonly ColumnId Gtin = new(nameof(ReceiptItemEntity.Gtin), Table);
    public static readonly ColumnId GtinNormalized = new(nameof(ReceiptItemEntity.GtinNormalized), Table);
    public static readonly ColumnId Label = new(nameof(ReceiptItemEntity.Label), Table);
    public static readonly ColumnId Number = new(nameof(ReceiptItemEntity.Number), Table);
    public static readonly ColumnId Price = new(nameof(ReceiptItemEntity.Price), Table);
    public static readonly ColumnId ProductId = new(nameof(ReceiptItemEntity.ProductId), Table);
    public static readonly ColumnId Quantity = new(nameof(ReceiptItemEntity.Quantity), Table);
    public static readonly ColumnId ReceiptId = new(nameof(ReceiptItemEntity.ReceiptId), Table);
    public static readonly ColumnId Sku = new(nameof(ReceiptItemEntity.Sku), Table);
    public static readonly ColumnId SkuNormalized = new(nameof(ReceiptItemEntity.SkuNormalized), Table);
    public static readonly ColumnId UnitPrice = new(nameof(ReceiptItemEntity.UnitPrice), Table);
  }

  public static class Receipts
  {
    public static readonly TableId Table = new(nameof(FakturContext.Receipts));

    public static readonly ColumnId AggregateId = new(nameof(ReceiptEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ReceiptEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ReceiptEntity.CreatedOn), Table);
    public static readonly ColumnId IssuedOn = new(nameof(ReceiptEntity.IssuedOn), Table);
    public static readonly ColumnId ItemCount = new(nameof(ReceiptEntity.ItemCount), Table);
    public static readonly ColumnId Number = new(nameof(ReceiptEntity.Number), Table);
    public static readonly ColumnId ReceiptId = new(nameof(ReceiptEntity.ReceiptId), Table);
    public static readonly ColumnId StoreId = new(nameof(ReceiptEntity.StoreId), Table);
    public static readonly ColumnId SubTotal = new(nameof(ReceiptEntity.SubTotal), Table);
    public static readonly ColumnId Total = new(nameof(ReceiptEntity.Total), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ReceiptEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ReceiptEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ReceiptEntity.Version), Table);
  }

  public static class ReceiptTaxes
  {
    public static readonly TableId Table = new(nameof(FakturContext.ReceiptTaxes));

    public static readonly ColumnId Amount = new(nameof(ReceiptTaxEntity.Amount), Table);
    public static readonly ColumnId Code = new(nameof(ReceiptTaxEntity.Code), Table);
    public static readonly ColumnId Rate = new(nameof(ReceiptTaxEntity.Rate), Table);
    public static readonly ColumnId ReceiptId = new(nameof(ReceiptTaxEntity.ReceiptId), Table);
    public static readonly ColumnId TaxableAmount = new(nameof(ReceiptTaxEntity.TaxableAmount), Table);
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
