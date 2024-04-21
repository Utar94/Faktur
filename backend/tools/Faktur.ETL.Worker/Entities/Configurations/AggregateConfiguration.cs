using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal abstract class AggregateConfiguration
{
  public virtual void Configure<T>(EntityTypeBuilder<T> builder) where T : AggregateEntity
  {
    builder.HasIndex(x => x.Deleted);
    builder.HasIndex(x => x.Key).IsUnique();

    builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
    builder.Property(x => x.Deleted).HasDefaultValue(false);
    builder.Property(x => x.Key).HasDefaultValueSql("uuid_generate_v4()");
    builder.Property(x => x.Version).HasDefaultValue(0);
  }
}
