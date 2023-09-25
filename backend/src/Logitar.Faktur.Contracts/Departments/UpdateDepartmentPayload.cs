namespace Logitar.Faktur.Contracts.Departments;

public record UpdateDepartmentPayload
{
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
