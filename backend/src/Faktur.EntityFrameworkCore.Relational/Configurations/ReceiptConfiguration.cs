﻿using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
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
    builder.HasIndex(x => x.SubTotal);
    builder.HasIndex(x => x.Total);
    builder.HasIndex(x => x.HasBeenProcessed);
    builder.HasIndex(x => x.ProcessedBy);
    builder.HasIndex(x => x.ProcessedOn);

    builder.Property(x => x.Number).HasMaxLength(NumberUnit.MaximumLength);
    builder.Property(x => x.SubTotal).HasColumnType("money");
    builder.Property(x => x.Total).HasColumnType("money");
    builder.Property(x => x.ProcessedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Store).WithMany(x => x.Receipts).OnDelete(DeleteBehavior.Restrict);
  }
}
