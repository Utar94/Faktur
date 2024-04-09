using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using FluentValidation.Results;
using Logitar;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Parsing;

internal class TsvReceiptParser : IReceiptParser
{
  private const char DepartmentFlag = '*';
  private const char DepartmentSeparator = '-';
  private const char ItemSeparator = '\t';

  public Task<IEnumerable<ReceiptItemUnit>> ExecuteAsync(string linesRaw, string propertyName, LocaleUnit? locale, CancellationToken cancellationToken)
  {
    string[] lines = linesRaw.Trim().Remove("\r").Split('\n');

    int capacity = lines.Length;
    List<ReceiptItemUnit> items = new(capacity);
    List<ValidationFailure> errors = new(capacity);

    NumberUnit? departmentNumber = null;
    DepartmentUnit? department = null;
    for (int i = 0; i < lines.Length; i++)
    {
      string indexedName = $"{propertyName}[{i}]";

      string line = lines[i].Trim();
      if (line.StartsWith(DepartmentFlag))
      {
        string[] values = line[1..].Split(DepartmentSeparator);
        if (values.Length != 2)
        {
          errors.Add(new ValidationFailure(indexedName, $"The department line does not have a valid column count (Expected=2, Actual={values.Length}).", line)
          {
            ErrorCode = "InvalidDepartmentLineColumnCount"
          });
          continue;
        }

        departmentNumber = new(values[0]); // TODO(fpion): validation?
        department = new(new DisplayNameUnit(values[1])); // TODO(fpion): validation?
      }
      else if (!string.IsNullOrEmpty(line))
      {
        string[] values = line.Split(ItemSeparator);
        if (values.Length != 4 && values.Length != 6)
        {
          errors.Add(new ValidationFailure(indexedName, $"The item line does not have a valid column count (Expected=4 or 6, Actual={values.Length}).", line)
          {
            ErrorCode = "InvalidItemLineColumnCount"
          });
          continue;
        }

        GtinUnit? gtin = null;
        SkuUnit? sku = null;
        string gtinOrSku = values[0].Trim();
        if (long.TryParse(gtinOrSku, out _))
        {
          gtin = new(gtinOrSku); // TODO(fpion): validation?
        }
        else
        {
          sku = new(gtinOrSku); // TODO(fpion): validation?
        }

        DisplayNameUnit label = new(values[1]); // TODO(fpion): validation?
        FlagsUnit? flags = FlagsUnit.TryCreate(values[2]); // TODO(fpion): validation?

        ValidationFailure? error = TryParsePrice(values[^1], $"{indexedName}.Price", locale, out decimal price);
        if (error != null)
        {
          errors.Add(error);
          continue;
        }

        double quantity = 1.0d;
        decimal unitPrice = price;
        if (values.Length == 6)
        {
          bool hasError = false;

          error = TryParseQuantity(values[3], $"{indexedName}.Quantity", locale, out quantity);
          if (error != null)
          {
            errors.Add(error);
            hasError = true;
          }

          error = TryParsePrice(values[4], $"{indexedName}.UnitPrice", locale, out unitPrice);
          if (error != null)
          {
            errors.Add(error);
            hasError = true;
          }

          if (hasError)
          {
            continue;
          }
        }

        ReceiptItemUnit item = new(gtin, sku, label, flags, quantity, unitPrice, price, departmentNumber, department);
        items.Add(item);
      }
    }

    if (errors.Count > 0)
    {
      throw new ValidationException(errors);
    }

    return Task.FromResult<IEnumerable<ReceiptItemUnit>>(items);
  }

  private static ValidationFailure? TryParseQuantity(string value, string propertyName, LocaleUnit? locale, out double quantity)
  {
    if (double.TryParse(value, NumberStyles.Any, locale?.Culture, out quantity) && quantity > 0d)
    {
      return null;
    }

    return new ValidationFailure(propertyName, "The specified value is not a valid quantity.", value)
    {
      ErrorCode = "InvalidQuantity"
    };
  }

  private static ValidationFailure? TryParsePrice(string value, string propertyName, LocaleUnit? locale, out decimal price)
  {
    if (decimal.TryParse(value, NumberStyles.Currency, locale?.Culture, out price) && price > 0.00m)
    {
      return null;
    }

    return new ValidationFailure(propertyName, "The specified value is not a valid price.", value)
    {
      ErrorCode = "InvalidPrice"
    };
  }
}
