using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Products;

public class SkuValidator : AbstractValidator<string>
{
  public SkuValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(SkuUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
