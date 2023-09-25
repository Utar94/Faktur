using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Departments;

public record DepartmentUnit // TODO(fpion): serialization
{
  public DepartmentNumberUnit Number { get; }
  public DisplayNameUnit DisplayName { get; }
  public DescriptionUnit? Description { get; }

  public DepartmentUnit(DepartmentNumberUnit number, DisplayNameUnit displayName, DescriptionUnit? description = null)
  {
    Number = number;
    DisplayName = displayName;
    Description = description;
  }
}
