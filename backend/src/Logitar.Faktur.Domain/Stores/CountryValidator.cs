using FluentValidation;

namespace Logitar.Faktur.Domain.Stores;

internal class CountryValidator : AbstractValidator<string>
{
  public CountryValidator()
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(AddressUnit.CountryMaximumLength)
      .Must(PostalAddressHelper.IsSupported)
        .WithErrorCode(nameof(CountryValidator))
        .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", PostalAddressHelper.SupportedCountries)}");
  }
}
