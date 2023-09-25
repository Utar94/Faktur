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
    builder.HasIndex(x => x.AddressFormatted);
    builder.HasIndex(x => x.PhoneE164Formatted);

    builder.Property(x => x.Number).HasMaxLength(StoreNumberUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Description).HasMaxLength(DescriptionUnit.MaximumLength);
    builder.Property(x => x.AddressStreet).HasMaxLength(AddressUnit.StreetMaximumLength);
    builder.Property(x => x.AddressLocality).HasMaxLength(AddressUnit.LocalityMaximumLength);
    builder.Property(x => x.AddressRegion).HasMaxLength(AddressUnit.RegionMaximumLength);
    builder.Property(x => x.AddressPostalCode).HasMaxLength(AddressUnit.PostalCodeMaximumLength);
    builder.Property(x => x.AddressCountry).HasMaxLength(AddressUnit.CountryMaximumLength);
    builder.Property(x => x.AddressFormatted).HasMaxLength(1000);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(PhoneUnit.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(PhoneUnit.NumberMaximumLength);
    builder.Property(x => x.PhoneExtension).HasMaxLength(PhoneUnit.ExtensionMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(20);
  }
}
