using Faktur.Infrastructure;

namespace Faktur.Relocation.Worker.Settings;

internal record DatabaseSettings
{
  public string ConnectionString { get; set; } = string.Empty;
  public DatabaseProvider DatabaseProvider { get; set; }
}
