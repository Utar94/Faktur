#nullable disable
using Faktur.Core.Articles;
using Faktur.Core.Products;
using Faktur.Core.Stores;
using Logitar.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Infrastructure
{
  public class FakturDbContext : IdentityDbContext
  {
    public FakturDbContext(DbContextOptions<FakturDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.ApplyConfigurationsFromAssembly(typeof(FakturDbContext).Assembly);
    }
  }
}
