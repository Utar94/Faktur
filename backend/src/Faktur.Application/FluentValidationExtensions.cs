using FluentValidation;

namespace Faktur.Application;

internal static class FluentValidationExtensions
{
  public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, string? propertyName)
  {
    return propertyName == null ? ruleBuilder : ruleBuilder.OverridePropertyName(propertyName).WithName(propertyName);
  }
}
