namespace Faktur.Contracts.Receipts;

public record DepartmentSummary
{
  public string Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public DepartmentSummary() : this(string.Empty, string.Empty)
  {
  }

  public DepartmentSummary(string number, string displayName)
  {
    Number = number;
    DisplayName = displayName;
  }
}
