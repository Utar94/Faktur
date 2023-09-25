using FluentValidation;

namespace Logitar.Faktur.Domain.Stores;

internal class RegionValidator : AbstractValidator<string>
{
  public RegionValidator(string country)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(AddressUnit.RegionMaximumLength);

    HashSet<string>? regions = PostalAddressHelper.GetCountry(country)?.Regions;
    if (regions != null)
    {
      RuleFor(x => x).Must(regions.Contains)
        .WithErrorCode(nameof(RegionValidator))
        .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", regions)}");
    }
  }
}
