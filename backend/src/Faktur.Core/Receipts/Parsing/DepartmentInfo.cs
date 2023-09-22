namespace Faktur.Core.Receipts.Parsing
{
  public class DepartmentInfo
  {
    public DepartmentInfo()
    {
      Name = string.Empty;
      Number = string.Empty;
    }

    public string Name { get; set; }
    public string Number { get; set; }
  }
}
