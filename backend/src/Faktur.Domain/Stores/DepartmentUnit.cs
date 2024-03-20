using Faktur.Domain.Shared;

namespace Faktur.Domain.Stores;

public record DepartmentUnit // TODO(fpion): move to Faktur.Domain.Stores
{
  public DisplayNameUnit DisplayName { get; }
  public DescriptionUnit? Description { get; }

  public DepartmentUnit(DisplayNameUnit displayName, DescriptionUnit? description = null)
  {
    DisplayName = displayName;
    Description = description;
  }
}
