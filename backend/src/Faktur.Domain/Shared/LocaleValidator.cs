using FluentValidation;

namespace Faktur.Domain.Shared;

public class LocaleValidator : AbstractValidator<string>
{
  private const int LOCALE_CUSTOM_UNSPECIFIED = 0x1000;
  private const int MaximumLength = 16;

  public LocaleValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(MaximumLength)
      .Must(BeAValidLocale)
        .WithErrorCode(nameof(LocaleValidator))
        .WithMessage("'{PropertyName}' may not be the invariant culture, nor a user-defined culture.")
      .WithPropertyName(propertyName);
  }

  private static bool BeAValidLocale(string name)
  {
    try
    {
      CultureInfo culture = CultureInfo.GetCultureInfo(name);
      return !string.IsNullOrEmpty(culture.Name) && culture.LCID != LOCALE_CUSTOM_UNSPECIFIED;
    }
    catch (CultureNotFoundException)
    {
      return false;
    }
  }
}
