using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.Faktur.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarFakturWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<FakturContext>(builder => builder.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("Logitar.Faktur.EntityFrameworkCore.SqlServer")
      ))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarFakturWithEntityFrameworkCoreRelational()
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
