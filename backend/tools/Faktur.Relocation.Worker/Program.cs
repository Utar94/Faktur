namespace Faktur.Relocation.Worker;

internal class Program
{
  private static void Main(string[] args)
  {
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    IHost host = builder.Build();
    host.Run();
  }
}
