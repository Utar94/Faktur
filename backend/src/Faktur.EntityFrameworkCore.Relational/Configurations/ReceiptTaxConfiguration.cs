﻿using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ReceiptTaxConfiguration : IEntityTypeConfiguration<ReceiptTaxEntity>
{
  public void Configure(EntityTypeBuilder<ReceiptTaxEntity> builder)
  {
    builder.ToTable(nameof(FakturContext.ReceiptTaxes));
    builder.HasKey(x => new { x.ReceiptId, x.Code });

    builder.HasIndex(x => x.Code);
    builder.HasIndex(x => x.Flags);
    builder.HasIndex(x => x.Rate);
    builder.HasIndex(x => x.TaxableAmount);
    builder.HasIndex(x => x.Amount);

    builder.Property(x => x.Flags).HasMaxLength(FlagsUnit.MaximumLength);
    builder.Property(x => x.Code).HasMaxLength(TaxCodeUnit.MaximumLength);
    builder.Property(x => x.TaxableAmount).HasColumnType("money");
    builder.Property(x => x.Amount).HasColumnType("money");

    builder.HasOne(x => x.Receipt).WithMany(x => x.Taxes).OnDelete(DeleteBehavior.Cascade);
  }
}
