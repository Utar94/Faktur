using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class ReceiptConfiguration : AggregateConfiguration, IEntityTypeConfiguration<ReceiptEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Receipts));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.Processed);

    builder.Property(x => x.IssuedAt).HasDefaultValueSql("now()");
    builder.Property(x => x.Number).HasMaxLength(32);
    builder.Property(x => x.Processed).HasDefaultValue(false);
    builder.Property(x => x.SubTotal).HasColumnType("money");
    builder.Property(x => x.Total).HasColumnType("money");

    builder.HasOne(x => x.Store).WithMany(x => x.Receipts).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.StoreId);
  }
}
