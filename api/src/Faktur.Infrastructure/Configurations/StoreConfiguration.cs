using Faktur.Core.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class StoreConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Store>
  {
    public void Configure(EntityTypeBuilder<Store> builder)
    {
      base.Configure(builder);

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
    }
  }
}
