using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities.Configurations;

internal class ArticleConfiguration : AggregateConfiguration, IEntityTypeConfiguration<ArticleEntity>
{
  public void Configure(EntityTypeBuilder<ArticleEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(LegacyContext.Articles));
    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Gtin);
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(100);
  }
}
