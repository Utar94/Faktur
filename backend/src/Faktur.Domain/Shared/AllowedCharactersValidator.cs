using FluentValidation;

namespace Faktur.Domain.Shared;

public class AllowedCharactersValidator : AbstractValidator<string>
{
  public AllowedCharactersValidator(string? allowedCharacters)
  {
    RuleFor(x => x).Must(s => allowedCharacters == null || s.All(allowedCharacters.Contains))
      .WithErrorCode(nameof(AllowedCharactersValidator))
      .WithMessage($"'{{PropertyName}}' may only contain the following characters: {allowedCharacters}");
  }
}
