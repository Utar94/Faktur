using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faktur.ETL.Worker.Entities;

internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
  public void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    builder.ToTable("AspNetUsers");
    builder.HasKey(x => x.Id);
  }
}
