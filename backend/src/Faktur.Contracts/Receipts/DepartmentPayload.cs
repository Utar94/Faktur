namespace Faktur.Contracts.Receipts;

public record DepartmentPayload
{
  public string Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public DepartmentPayload() : this(string.Empty, string.Empty)
  {
  }

  public DepartmentPayload(string number, string displayName)
  {
    Number = number;
    DisplayName = displayName;
  }
}
