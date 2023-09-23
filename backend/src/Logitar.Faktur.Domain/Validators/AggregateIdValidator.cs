using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Validators;

public class AggregateIdValidator : AbstractValidator<string>
{
  public AggregateIdValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(AggregateId.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
