using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ProductConfiguration : AggregateConfiguration<ProductEntity>, IEntityTypeConfiguration<ProductEntity>
{
  public override void Configure(EntityTypeBuilder<ProductEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Products));
    builder.HasKey(x => x.ProductId);

    builder.HasIndex(x => new { x.StoreId, x.ArticleId }).IsUnique();
    builder.HasIndex(x => new { x.StoreId, x.Sku }).IsUnique();
    builder.HasIndex(x => x.Sku);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Flags);
    builder.HasIndex(x => x.UnitPrice);
    builder.HasIndex(x => x.UnitType);

    builder.Property(x => x.Sku).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.SkuNormalized).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Flags).HasMaxLength(FlagsUnit.MaximumLength);
    builder.Property(x => x.UnitPrice).HasColumnType("money");
    builder.Property(x => x.UnitType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<UnitType>());

    builder.HasOne(x => x.Article).WithMany(x => x.Products).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Department).WithMany(x => x.Products).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Store).WithMany(x => x.Products).OnDelete(DeleteBehavior.Restrict);
  }
}
