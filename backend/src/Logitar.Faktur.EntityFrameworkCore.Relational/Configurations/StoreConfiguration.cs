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

    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Description).HasMaxLength(DescriptionUnit.MaximumLength);
  }
}
