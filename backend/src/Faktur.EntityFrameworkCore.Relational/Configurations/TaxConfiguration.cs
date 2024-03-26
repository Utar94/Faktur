using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class TaxConfiguration : AggregateConfiguration<TaxEntity>, IEntityTypeConfiguration<TaxEntity>
{
  public override void Configure(EntityTypeBuilder<TaxEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Taxes));
    builder.HasKey(x => x.TaxId);

    builder.HasIndex(x => x.Code);
    builder.HasIndex(x => x.CodeNormalized).IsUnique();
    builder.HasIndex(x => x.Rate);
    builder.HasIndex(x => x.Flags);

    builder.Property(x => x.Code).HasMaxLength(TaxCodeUnit.MaximumLength);
    builder.Property(x => x.CodeNormalized).HasMaxLength(TaxCodeUnit.MaximumLength);
    builder.Property(x => x.Flags).HasMaxLength(FlagsUnit.MaximumLength);
  }
}
