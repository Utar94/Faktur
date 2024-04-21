using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class ReceiptItemConfiguration : IEntityTypeConfiguration<ReceiptItemEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptItemEntity> builder)
  {
    builder.ToTable(nameof(LegacyContext.ReceiptItems));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Key).IsUnique();

    builder.Property(x => x.Price).HasColumnType("money");
    builder.Property(x => x.UnitPrice).HasColumnType("money");

    builder.HasOne(x => x.Product).WithMany(x => x.ReceiptItems).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.ProductId);
    builder.HasOne(x => x.Receipt).WithMany(x => x.Items).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.ReceiptId);
  }
}
