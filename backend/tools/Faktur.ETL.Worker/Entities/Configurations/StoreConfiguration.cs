using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class StoreConfiguration : AggregateConfiguration, IEntityTypeConfiguration<StoreEntity>
{
  public void Configure(EntityTypeBuilder<StoreEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Stores));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Number);

    builder.Property(x => x.Address).HasMaxLength(100);
    builder.Property(x => x.City).HasMaxLength(100);
    builder.Property(x => x.Country).HasMaxLength(2).IsFixedLength();
    builder.Property(x => x.Name).HasMaxLength(100);
    builder.Property(x => x.Number).HasMaxLength(32);
    builder.Property(x => x.Phone).HasMaxLength(40);
    builder.Property(x => x.PostalCode).HasMaxLength(10);
    builder.Property(x => x.State).HasMaxLength(2).IsFixedLength();

    builder.HasOne(x => x.Banner).WithMany(x => x.Stores).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.BannerId);
  }
}
