using Faktur.Core.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.Infrastructure.Configurations
{
  internal class ArticleConfiguration : AggregateConfiguration, IEntityTypeConfiguration<Article>
  {
    public void Configure(EntityTypeBuilder<Article> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Gtin);
      builder.HasIndex(x => x.Name);

      builder.Property(x => x.Name).HasMaxLength(100);
    }
  }
}
