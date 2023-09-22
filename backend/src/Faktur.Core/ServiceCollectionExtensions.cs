using Faktur.Core.Receipts.Parsing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Faktur.Core
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      return services
        .AddAutoMapper(assembly)
        .AddMediatR(assembly)
        .AddSingleton<IReceiptParser, TsvReceiptParser>();
    }
  }
}
