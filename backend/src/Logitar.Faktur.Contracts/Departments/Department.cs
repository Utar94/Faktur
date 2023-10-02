using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Contracts.Departments;

public class Department : Metadata
{
  public string Number { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public Store? Store { get; set; }
}
