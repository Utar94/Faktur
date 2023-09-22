using Faktur.Core.Receipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class ItemConfiguration : IEntityTypeConfiguration<Item>
  {
    public void Configure(EntityTypeBuilder<Item> builder)
    {
      builder.ToTable("ReceiptItems");

      builder.HasIndex(x => x.Key).IsUnique();

      builder.Property(x => x.Price).HasColumnType("money");
      builder.Property(x => x.UnitPrice).HasColumnType("money");
    }
  }
}
