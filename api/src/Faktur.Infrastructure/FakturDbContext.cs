#nullable disable
using Faktur.Core;
using Faktur.Core.Articles;
using Faktur.Core.Products;
using Faktur.Core.Receipts;
using Faktur.Core.Stores;
using Logitar.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Infrastructure
{
  public class FakturDbContext : IdentityDbContext, IDbContext
  {
    public FakturDbContext(DbContextOptions<FakturDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Line> Lines { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Tax> Taxes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.ApplyConfigurationsFromAssembly(typeof(FakturDbContext).Assembly);
    }
  }
}
