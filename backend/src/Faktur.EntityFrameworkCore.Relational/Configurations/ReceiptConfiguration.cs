using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ReceiptConfiguration : AggregateConfiguration<ReceiptEntity>, IEntityTypeConfiguration<ReceiptEntity>
{
  public override void Configure(EntityTypeBuilder<ReceiptEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Receipts));
    builder.HasKey(x => x.ReceiptId);

    builder.HasIndex(x => x.IssuedOn);
    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.ItemCount);

    builder.Property(x => x.Number).HasMaxLength(NumberUnit.MaximumLength);

    builder.HasOne(x => x.Store).WithMany(x => x.Receipts).OnDelete(DeleteBehavior.Restrict);
  }
}
