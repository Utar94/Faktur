using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Entities;

internal class LegacyContext : DbContext
{
  public LegacyContext(DbContextOptions<LegacyContext> options) : base(options)
  {
  }

  public DbSet<ArticleEntity> Articles { get; private set; }
  public DbSet<BannerEntity> Banners { get; private set; }
  public DbSet<DepartmentEntity> Departments { get; private set; }
  public DbSet<ProductEntity> Products { get; private set; }
  public DbSet<ReceiptItemEntity> ReceiptItems { get; private set; }
  public DbSet<ReceiptLineEntity> ReceiptLines { get; private set; }
  public DbSet<ReceiptTaxEntity> ReceiptTaxes { get; private set; }
  public DbSet<ReceiptEntity> Receipts { get; private set; }
  public DbSet<StoreEntity> Stores { get; private set; }
  public DbSet<UserEntity> Users { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
