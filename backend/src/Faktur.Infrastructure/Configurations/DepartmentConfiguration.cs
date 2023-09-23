using Faktur.Core.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class DepartmentConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Department>
  {
    public void Configure(EntityTypeBuilder<Department> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Name);
      builder.HasIndex(x => x.Number);

      builder.Property(x => x.Name).HasMaxLength(100);
      builder.Property(x => x.Number).HasMaxLength(32);
    }
  }
}
