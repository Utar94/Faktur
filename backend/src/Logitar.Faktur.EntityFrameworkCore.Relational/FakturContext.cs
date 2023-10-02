using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational;

public class FakturContext : DbContext
{
  public FakturContext(DbContextOptions<FakturContext> options) : base(options)
  {
  }

  internal DbSet<ActorEntity> Actors { get; private set; }
  internal DbSet<ArticleEntity> Articles { get; private set; }
  internal DbSet<BannerEntity> Banners { get; private set; }
  internal DbSet<DepartmentEntity> Departments { get; private set; }
  internal DbSet<ProductEntity> Products { get; private set; }
  internal DbSet<StoreEntity> Stores { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(FakturContext).Assembly);
  }
}
