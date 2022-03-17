#nullable disable
using Logitar.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Infrastructure
{
  public class FakturDbContext : IdentityDbContext
  {
    public FakturDbContext(DbContextOptions<FakturDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
