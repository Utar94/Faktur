using FluentValidation;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Domain.Stores;

public class AddressValidator : AbstractValidator<IAddress>
{
  public AddressValidator()
  {
    RuleFor(x => x.Street).NotEmpty()
      .MaximumLength(AddressUnit.StreetMaximumLength);

    RuleFor(x => x.Locality).NotEmpty()
      .MaximumLength(AddressUnit.StreetMaximumLength);

    When(x => !string.IsNullOrWhiteSpace(x.Region),
      () => RuleFor(x => x.Region!).SetValidator(x => new RegionValidator(x.Country))
    ).Otherwise(() => When(x => PostalAddressHelper.GetCountry(x.Country)?.Regions != null,
      () => RuleFor(x => x.Region).NotEmpty()
    ));

    When(x => !string.IsNullOrWhiteSpace(x.PostalCode),
      () => RuleFor(x => x.PostalCode!).SetValidator(x => new PostalCodeValidator(x.Country))
    ).Otherwise(() => When(x => PostalAddressHelper.GetCountry(x.Country)?.PostalCode != null,
      () => RuleFor(x => x.PostalCode).NotEmpty()
    ));

    RuleFor(x => x.Country).SetValidator(new CountryValidator());
  }
}
