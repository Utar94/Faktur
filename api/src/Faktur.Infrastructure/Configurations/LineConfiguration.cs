using Faktur.Core.Receipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class LineConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Line>
  {
    public void Configure(EntityTypeBuilder<Line> builder)
    {
      base.Configure(builder);

      builder.ToTable("ReceiptLines");

      builder.HasIndex(x => x.ItemId).IsUnique();
      builder.HasIndex(x => x.ReceiptId);

      builder.Property(x => x.ArticleName).HasMaxLength(100);
      builder.Property(x => x.BannerName).HasMaxLength(100);
      builder.Property(x => x.Category).HasMaxLength(100);
      builder.Property(x => x.DepartmentName).HasMaxLength(100);
      builder.Property(x => x.DepartmentNumber).HasMaxLength(32);
      builder.Property(x => x.Flags).HasMaxLength(10);
      builder.Property(x => x.Price).HasColumnType("money");
      builder.Property(x => x.ProductLabel).HasMaxLength(100);
      builder.Property(x => x.ReceiptNumber).HasMaxLength(32);
      builder.Property(x => x.Sku).HasMaxLength(32);
      builder.Property(x => x.StoreName).HasMaxLength(100);
      builder.Property(x => x.StoreNumber).HasMaxLength(32);
      builder.Property(x => x.UnitPrice).HasColumnType("money");
      builder.Property(x => x.UnitType).HasMaxLength(4);
    }
  }
}
