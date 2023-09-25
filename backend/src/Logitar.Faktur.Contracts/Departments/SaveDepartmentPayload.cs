namespace Logitar.Faktur.Contracts.Departments;

public record SaveDepartmentPayload
{
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
