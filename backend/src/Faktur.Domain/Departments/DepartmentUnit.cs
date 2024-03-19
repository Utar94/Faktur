﻿using Faktur.Domain.Shared;

namespace Faktur.Domain.Departments;

public record DepartmentUnit
{
  public DisplayNameUnit DisplayName { get; }
  public DescriptionUnit? Description { get; }

  public DepartmentUnit(DisplayNameUnit displayName, DescriptionUnit? description = null)
  {
    DisplayName = displayName;
    Description = description;
  }
}