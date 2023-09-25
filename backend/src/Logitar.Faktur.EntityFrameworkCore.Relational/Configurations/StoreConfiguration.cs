using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Configurations;

internal class StoreConfiguration : AggregateConfiguration<StoreEntity>, IEntityTypeConfiguration<StoreEntity>
{
  public override void Configure(EntityTypeBuilder<StoreEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Stores));
    builder.HasKey(x => x.StoreId);

    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.NumberNormalized);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.PhoneE164Formatted);

    builder.Property(x => x.Number).HasMaxLength(StoreNumberUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Description).HasMaxLength(DescriptionUnit.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(PhoneUnit.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(PhoneUnit.NumberMaximumLength);
    builder.Property(x => x.PhoneExtension).HasMaxLength(PhoneUnit.ExtensionMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(12);
  }
}
