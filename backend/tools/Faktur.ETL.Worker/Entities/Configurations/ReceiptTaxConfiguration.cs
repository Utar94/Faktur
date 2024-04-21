using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class ReceiptTaxConfiguration : IEntityTypeConfiguration<ReceiptTaxEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptTaxEntity> builder)
  {
    builder.ToTable(nameof(LegacyContext.ReceiptTaxes));
    builder.HasKey(x => new { x.ReceiptId, x.Code });

    builder.Property(x => x.Amount).HasColumnType("money");
    builder.Property(x => x.Code).HasMaxLength(4);
    builder.Property(x => x.TaxableAmount).HasColumnType("money");

    builder.HasOne(x => x.Receipt).WithMany(x => x.Taxes).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.ReceiptId);
  }
}
