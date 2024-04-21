﻿namespace Faktur.ETL.Worker;

internal class Program
{
  internal static void Main(string[] args)
  {
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    Startup startup = new();
    startup.ConfigureServices(builder.Services);

    IHost host = builder.Build();
    host.Run();
  }
}
