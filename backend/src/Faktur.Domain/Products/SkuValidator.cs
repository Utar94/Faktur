using FluentValidation;

namespace Faktur.Domain.Products;

public class SkuValidator : AbstractValidator<string>
{
  public SkuValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(SkuUnit.MaximumLength);
  }
}
