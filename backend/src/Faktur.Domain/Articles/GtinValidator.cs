using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Articles;

public class GtinValidator : AbstractValidator<string>
{
  public GtinValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(GtinUnit.MaximumLength).SetValidator(new AllowedCharactersValidator("0123456789"));
  }
}
