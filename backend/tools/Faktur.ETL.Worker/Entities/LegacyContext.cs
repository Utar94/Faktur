using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Entities;

internal class LegacyContext : DbContext
{
  public LegacyContext(DbContextOptions<LegacyContext> options) : base(options)
  {
  }

  public DbSet<UserEntity> Users { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
