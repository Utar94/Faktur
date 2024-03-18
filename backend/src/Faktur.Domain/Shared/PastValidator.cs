using FluentValidation;

namespace Faktur.Domain.Shared;

public class PastValidator : AbstractValidator<DateTime>
{
  public PastValidator(DateTime? moment = null)
  {
    RuleFor(x => x).Must(value => value < (moment ?? DateTime.Now))
      .WithErrorCode(nameof(PastValidator))
      .WithMessage("'{PropertyName}' must be a date and time set in the past.");
  }
}
