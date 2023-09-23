using Faktur.Core.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class ProductConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Product>
  {
    public void Configure(EntityTypeBuilder<Product> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Label);
      builder.HasIndex(x => new { x.StoreId, x.ArticleId }).IsUnique();
      builder.HasIndex(x => new { x.StoreId, x.Sku }).IsUnique();

      builder.Property(x => x.Flags).HasMaxLength(10);
      builder.Property(x => x.Label).HasMaxLength(100);
      builder.Property(x => x.Sku).HasMaxLength(32);
      builder.Property(x => x.UnitPrice).HasColumnType("money");
      builder.Property(x => x.UnitType).HasMaxLength(4);
    }
  }
}
