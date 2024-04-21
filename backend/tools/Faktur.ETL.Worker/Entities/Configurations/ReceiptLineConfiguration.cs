using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class LineConfiguration : AggregateConfiguration, IEntityTypeConfiguration<ReceiptLineEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptLineEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.ReceiptLines));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.ItemId).IsUnique();

    builder.Property(x => x.Category).HasMaxLength(100);

    builder.HasOne(x => x.Item).WithOne(x => x.Line).HasPrincipalKey<ReceiptItemEntity>(x => x.Id).HasForeignKey<ReceiptLineEntity>(x => x.ItemId);
  }
}
