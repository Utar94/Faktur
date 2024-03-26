using Faktur.Application.Articles;
using Faktur.Application.Banners;
using Faktur.Application.Departments;
using Faktur.Application.Products;
using Faktur.Application.Receipts;
using Faktur.Application.Stores;
using Faktur.Application.Taxes;
using Faktur.Domain.Articles;
using Faktur.Domain.Banners;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Queriers;
using Faktur.EntityFrameworkCore.Relational.Repositories;
using Faktur.Infrastructure;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddFakturWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddFakturInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IArticleQuerier, ArticleQuerier>()
      .AddTransient<IBannerQuerier, BannerQuerier>()
      .AddTransient<IDepartmentQuerier, DepartmentQuerier>()
      .AddTransient<IProductQuerier, ProductQuerier>()
      .AddTransient<IReceiptItemQuerier, ReceiptItemQuerier>()
      .AddTransient<IReceiptQuerier, ReceiptQuerier>()
      .AddTransient<IStoreQuerier, StoreQuerier>()
      .AddTransient<ITaxQuerier, TaxQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IArticleRepository, ArticleRepository>()
      .AddTransient<IBannerRepository, BannerRepository>()
      .AddTransient<IProductRepository, ProductRepository>()
      .AddTransient<IReceiptRepository, ReceiptRepository>()
      .AddTransient<IStoreRepository, StoreRepository>()
      .AddTransient<ITaxRepository, TaxRepository>();
  }
}
