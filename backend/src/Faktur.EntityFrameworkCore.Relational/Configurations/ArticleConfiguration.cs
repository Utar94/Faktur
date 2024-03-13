using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.EntityFrameworkCore.Relational.Configurations;

internal class ArticleConfiguration : AggregateConfiguration<ArticleEntity>, IEntityTypeConfiguration<ArticleEntity>
{
  public override void Configure(EntityTypeBuilder<ArticleEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(FakturContext.Articles));
    builder.HasKey(x => x.ArticleId);

    builder.HasIndex(x => x.Gtin);
    builder.HasIndex(x => x.GtinNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.Gtin).HasMaxLength(GtinUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
  }
}
