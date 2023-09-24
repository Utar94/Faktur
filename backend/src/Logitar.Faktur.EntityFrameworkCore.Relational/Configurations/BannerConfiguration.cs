using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Configurations;

internal class BannerConfiguration : AggregateConfiguration<BannerEntity>, IEntityTypeConfiguration<BannerEntity>
{
  public override void Configure(EntityTypeBuilder<BannerEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Banners));
    builder.HasKey(x => x.BannerId);

    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Description).HasMaxLength(DescriptionUnit.MaximumLength);
  }
}
