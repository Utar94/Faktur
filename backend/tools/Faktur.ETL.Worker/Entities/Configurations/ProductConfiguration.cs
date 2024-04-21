using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class ProductConfiguration : AggregateConfiguration, IEntityTypeConfiguration<ProductEntity>
{
  public void Configure(EntityTypeBuilder<ProductEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Products));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Label);
    builder.HasIndex(x => new { x.StoreId, x.ArticleId }).IsUnique();
    builder.HasIndex(x => new { x.StoreId, x.Sku }).IsUnique();

    builder.Property(x => x.Flags).HasMaxLength(10);
    builder.Property(x => x.Label).HasMaxLength(100);
    builder.Property(x => x.Sku).HasMaxLength(32);
    builder.Property(x => x.UnitPrice).HasColumnType("money");
    builder.Property(x => x.UnitType).HasMaxLength(4);

    builder.HasOne(x => x.Article).WithMany(x => x.Products).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.ArticleId);
    builder.HasOne(x => x.Department).WithMany(x => x.Products).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.DepartmentId);
    builder.HasOne(x => x.Store).WithMany(x => x.Products).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.StoreId);
  }
}
