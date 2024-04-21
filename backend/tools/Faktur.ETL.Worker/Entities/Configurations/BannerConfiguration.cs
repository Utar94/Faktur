using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class BannerConfiguration : AggregateConfiguration, IEntityTypeConfiguration<BannerEntity>
{
  public void Configure(EntityTypeBuilder<BannerEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Banners));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(100);
  }
}
