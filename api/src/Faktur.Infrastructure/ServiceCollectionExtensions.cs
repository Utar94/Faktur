using Faktur.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Infrastructure
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
      return services
        .AddDbContext<IDbContext, FakturDbContext>(ConfigureDbContext)
        .AddDbContext<FakturDbContext>(ConfigureDbContext);
    }

    private static void ConfigureDbContext(IServiceProvider provider, DbContextOptionsBuilder builder)
    {
      var configuration = provider.GetRequiredService<IConfiguration>();
      builder.UseNpgsql(configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(FakturDbContext)}"));
    }
  }
}
