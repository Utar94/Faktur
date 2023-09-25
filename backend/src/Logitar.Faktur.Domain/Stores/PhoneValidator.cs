using FluentValidation;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Stores;

public class PhoneValidator : AbstractValidator<IPhone>
{
  public PhoneValidator(string? propertyName = null)
  {
    When(x => x.CountryCode != null,
      () => RuleFor(x => x.CountryCode).NotEmpty()
        .MaximumLength(PhoneUnit.CountryCodeMaximumLength));

    RuleFor(x => x.Number).NotEmpty()
      .MaximumLength(PhoneUnit.NumberMaximumLength);

    When(x => x.Extension != null,
      () => RuleFor(x => x.Extension).NotEmpty()
        .MaximumLength(PhoneUnit.ExtensionMaximumLength));

    RuleFor(x => x).Must(phone => phone.IsValid())
      .WithErrorCode("PhoneValidator")
      .WithMessage("'{PropertyName}' must be a valid phone number.")
      .WithPropertyName(propertyName);
  }
}
