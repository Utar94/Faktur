using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Receipts;

public class IssuedOnValidator : AbstractValidator<DateTime>
{
  public IssuedOnValidator(string? propertyName = null)
  {
    RuleFor(x => x).SetValidator(new PastValidator()).WithPropertyName(propertyName);
  }
}
