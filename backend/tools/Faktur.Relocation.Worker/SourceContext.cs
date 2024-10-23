using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker;

internal class SourceContext : DbContext
{
  public SourceContext(DbContextOptions<SourceContext> options) : base(options)
  {
  }

  internal DbSet<EventEntity> Events { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new EventConfiguration());
  }
}
