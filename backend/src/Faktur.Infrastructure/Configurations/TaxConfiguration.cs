using Faktur.Core.Receipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class TaxConfiguration : IEntityTypeConfiguration<Tax>
  {
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
      builder.ToTable("ReceiptTaxes");

      builder.HasKey(x => new { x.ReceiptId, x.Code });

      builder.Property(x => x.Amount).HasColumnType("money");
      builder.Property(x => x.Code).HasMaxLength(4);
      builder.Property(x => x.TaxableAmount).HasColumnType("money");
    }
  }
}
