namespace Faktur.Contracts.Departments;

public record CreateOrReplaceDepartmentPayload
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateOrReplaceDepartmentPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceDepartmentPayload(string displayName)
  {
    DisplayName = displayName;
  }
}
