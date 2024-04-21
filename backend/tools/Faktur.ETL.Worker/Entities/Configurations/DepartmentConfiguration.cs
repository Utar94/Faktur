using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class DepartmentConfiguration : AggregateConfiguration, IEntityTypeConfiguration<DepartmentEntity>
{
  public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Departments));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Number);

    builder.Property(x => x.Name).HasMaxLength(100);
    builder.Property(x => x.Number).HasMaxLength(32);

    builder.HasOne(x => x.Store).WithMany(x => x.Departments).HasPrincipalKey(x => x.Id).HasForeignKey(x => x.StoreId);
  }
}
