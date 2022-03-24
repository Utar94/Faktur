using Faktur.Core.Receipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class ReceiptConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Receipt>
  {
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Number);
      builder.HasIndex(x => x.Processed);

      builder.Property(x => x.IssuedAt).HasDefaultValueSql("now()");
      builder.Property(x => x.Number).HasMaxLength(32);
      builder.Property(x => x.Processed).HasDefaultValue(false);
      builder.Property(x => x.SubTotal).HasColumnType("money");
      builder.Property(x => x.Total).HasColumnType("money");
    }
  }
}
