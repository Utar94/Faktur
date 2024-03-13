using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class StoreConfiguration : AggregateConfiguration<StoreEntity>, IEntityTypeConfiguration<StoreEntity>
{
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

    builder.HasOne(x => x.Banner).WithMany(x => x.Stores).OnDelete(DeleteBehavior.Restrict);
  }
}
