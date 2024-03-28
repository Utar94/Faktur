using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ReceiptItemConfiguration : IEntityTypeConfiguration<ReceiptItemEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptItemEntity> builder)
  {
    builder.ToTable(nameof(FakturContext.ReceiptItems));
    builder.HasKey(x => new { x.ReceiptId, x.Number });

    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.Gtin);
    builder.HasIndex(x => x.GtinNormalized);
    builder.HasIndex(x => x.Sku);
    builder.HasIndex(x => x.SkuNormalized);
    builder.HasIndex(x => x.Label);
    builder.HasIndex(x => x.Flags);
    builder.HasIndex(x => x.Quantity);
    builder.HasIndex(x => x.UnitPrice);
    builder.HasIndex(x => x.Price);
    builder.HasIndex(x => x.DepartmentNumber);
    builder.HasIndex(x => x.DepartmentName);
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.Gtin).HasMaxLength(GtinUnit.MaximumLength);
    builder.Property(x => x.Sku).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.SkuNormalized).HasMaxLength(SkuUnit.MaximumLength);
    builder.Property(x => x.Label).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Flags).HasMaxLength(FlagsUnit.MaximumLength);
    builder.Property(x => x.UnitPrice).HasColumnType("money");
    builder.Property(x => x.Price).HasColumnType("money");
    builder.Property(x => x.DepartmentNumber).HasMaxLength(NumberUnit.MaximumLength);
    builder.Property(x => x.DepartmentName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Category).HasMaxLength(CategoryUnit.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Receipt).WithMany(x => x.Items).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Product).WithMany(x => x.ReceiptItems).OnDelete(DeleteBehavior.SetNull);
  }
}
