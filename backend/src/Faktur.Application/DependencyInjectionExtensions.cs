using Faktur.Application.Receipts.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddFakturApplication(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<IReceiptParser, TsvReceiptParser>();
  }
}
