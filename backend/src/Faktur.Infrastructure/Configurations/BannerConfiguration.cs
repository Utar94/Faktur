using Faktur.Core.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class BannerConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Banner>
  {
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Name);

      builder.Property(x => x.Name).HasMaxLength(100);
    }
  }
}
