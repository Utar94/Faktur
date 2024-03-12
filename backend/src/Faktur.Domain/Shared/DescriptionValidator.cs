using FluentValidation;

namespace Faktur.Domain.Shared;

public class DescriptionValidator : AbstractValidator<string>
{
  public DescriptionValidator()
  {
    RuleFor(x => x).NotEmpty();
  }
}
