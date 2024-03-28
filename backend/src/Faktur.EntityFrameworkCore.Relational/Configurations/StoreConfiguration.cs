using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class StoreConfiguration : AggregateConfiguration<StoreEntity>, IEntityTypeConfiguration<StoreEntity>
{
  private const int AddressFormattedMaximumLength = AddressUnit.MaximumLength * 5 + 4; // NOTE(fpion): enough space to contain the five address components, each separated by one character.
  private const int PhoneE164FormattedMaximumLength = PhoneUnit.CountryCodeMaximumLength + 1 + PhoneUnit.NumberMaximumLength + 7 + PhoneUnit.ExtensionMaximumLength; // NOTE(fpion): enough space to contain the following format '{CountryCode} {Number}, ext. {Extension}'.

  public override void Configure(EntityTypeBuilder<StoreEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Stores));
    builder.HasKey(x => x.StoreId);

    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.DepartmentCount);

    builder.Property(x => x.Number).HasMaxLength(NumberUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.AddressStreet).HasMaxLength(AddressUnit.MaximumLength);
    builder.Property(x => x.AddressLocality).HasMaxLength(AddressUnit.MaximumLength);
    builder.Property(x => x.AddressPostalCode).HasMaxLength(AddressUnit.MaximumLength);
    builder.Property(x => x.AddressRegion).HasMaxLength(AddressUnit.MaximumLength);
    builder.Property(x => x.AddressCountry).HasMaxLength(AddressUnit.MaximumLength);
    builder.Property(x => x.AddressFormatted).HasMaxLength(AddressFormattedMaximumLength);
    builder.Property(x => x.AddressVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(EmailUnit.MaximumLength);
    builder.Property(x => x.EmailVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(PhoneUnit.CountryCodeMaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(PhoneUnit.NumberMaximumLength);
    builder.Property(x => x.PhoneExtension).HasMaxLength(PhoneUnit.ExtensionMaximumLength);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(PhoneE164FormattedMaximumLength);
    builder.Property(x => x.PhoneVerifiedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Banner).WithMany(x => x.Stores).OnDelete(DeleteBehavior.Restrict);
  }
}
