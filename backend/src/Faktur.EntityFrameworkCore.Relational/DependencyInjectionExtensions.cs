using Faktur.Application.Articles;
using Faktur.Application.Banners;
using Faktur.Application.Departments;
using Faktur.Application.Stores;
using Faktur.Domain.Articles;
using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Queriers;
using Faktur.EntityFrameworkCore.Relational.Repositories;
using Faktur.Infrastructure;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddFakturWithEntityFrameworkCoreRelational(this IServiceCollection services, IConfiguration configuration)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddFakturInfrastructure(configuration)
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
      .AddTransient<IStoreQuerier, StoreQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IArticleRepository, ArticleRepository>()
      .AddTransient<IBannerRepository, BannerRepository>()
      .AddTransient<IStoreRepository, StoreRepository>();
  }
}
