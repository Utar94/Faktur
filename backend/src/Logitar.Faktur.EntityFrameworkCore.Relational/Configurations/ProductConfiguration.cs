using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
  public void Configure(EntityTypeBuilder<ProductEntity> builder)
  {
    builder.ToTable(nameof(FakturContext.Products));
    builder.HasKey(x => x.ProductId);

    builder.HasIndex(x => new { x.StoreId, x.ArticleId }).IsUnique();
    builder.HasIndex(x => x.Sku);
    builder.HasIndex(x => new { x.StoreId, x.SkuNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Flags);
    builder.HasIndex(x => x.UnitPrice);
    builder.HasIndex(x => x.UnitType);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.HasOne(x => x.Article).WithMany(x => x.Products).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Department).WithMany(x => x.Products).OnDelete(DeleteBehavior.NoAction);
    builder.HasOne(x => x.Store).WithMany(x => x.Products).OnDelete(DeleteBehavior.Cascade);

    builder.Property(x => x.Sku).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.SkuNormalized).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Description).HasMaxLength(DescriptionUnit.MaximumLength);
    builder.Property(x => x.Flags).HasMaxLength(FlagsUnit.MaximumLength);
    builder.Property(x => x.UnitType).HasMaxLength(UnitTypeUnit.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
