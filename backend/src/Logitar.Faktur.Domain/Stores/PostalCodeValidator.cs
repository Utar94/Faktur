using FluentValidation;

namespace Logitar.Faktur.Domain.Stores;

internal class PostalCodeValidator : AbstractValidator<string>
{
  public PostalCodeValidator(string country)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(AddressUnit.PostalCodeMaximumLength);

    string? expression = PostalAddressHelper.GetCountry(country)?.PostalCode;
    if (expression != null)
    {
      RuleFor(x => x).Matches(expression)
        .WithErrorCode(nameof(PostalCodeValidator))
        .WithMessage($"'{{PropertyName}}' must match the following expression: {expression}");
    }
  }
}
