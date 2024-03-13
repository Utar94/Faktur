using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentEntity>
{
  public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
  {
    builder.ToTable(nameof(FakturContext.Departments));
    builder.HasKey(x => x.DepartmentId);

    builder.HasIndex(x => new { x.StoreId, x.NumberNormalized }).IsUnique();
    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.Number).HasMaxLength(NumberUnit.MaximumLength);
    builder.Property(x => x.NumberNormalized).HasMaxLength(NumberUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Store).WithMany(x => x.Departments).OnDelete(DeleteBehavior.Cascade);
  }
}
