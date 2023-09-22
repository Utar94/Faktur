using Faktur.Core.Articles;
using Faktur.Core.Products;
using Faktur.Core.Receipts;
using Faktur.Core.Stores;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core
{
  public interface IDbContext
  {
    DbSet<Article> Articles { get; }
    DbSet<Banner> Banners { get; }
    DbSet<Department> Departments { get; }
    DbSet<Item> Items { get; }
    DbSet<Line> Lines { get; }
    DbSet<Product> Products { get; }
    DbSet<Receipt> Receipts { get; }
    DbSet<Store> Stores { get; }
    DbSet<Tax> Taxes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}
