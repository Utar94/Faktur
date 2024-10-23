using FluentValidation;

namespace Faktur.Domain.Receipts;

public class ReceiptItemValidator : AbstractValidator<ReceiptItemUnit>
{
  public ReceiptItemValidator()
  {
    RuleFor(x => x).Must(x => x.Gtin != null || x.Sku != null).WithErrorCode(nameof(ReceiptItemValidator))
      .WithMessage(x => $"At least one of the following must be provided: {nameof(x.Gtin)}, {nameof(x.Sku)}.");

    RuleFor(x => x.Quantity).GreaterThan(0);
    //RuleFor(x => x.UnitPrice).GreaterThan(0);
    //RuleFor(x => x.Price).GreaterThan(0);

    When(x => x.DepartmentNumber != null || x.Department != null, () =>
    {
      RuleFor(x => x.DepartmentNumber).NotNull();
      RuleFor(x => x.Department).NotNull();
    });
  }
}
