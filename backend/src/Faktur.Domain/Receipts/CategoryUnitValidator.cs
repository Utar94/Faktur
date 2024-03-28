using FluentValidation;

namespace Faktur.Domain.Receipts;

public class CategoryUnitValidator : AbstractValidator<string>
{
  public CategoryUnitValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(CategoryUnit.MaximumLength);
  }
}
