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

    List<ReceiptItemUnit> items = new(capacity: lines.Length);
    List<ValidationFailure> errors = [];

    DepartmentInfo? department = null;
    for (int i = 0; i < lines.Length; i++)
    {
      string indexedName = $"{propertyName}[{i}]";

      string line = lines[i].Trim();
      if (IsDepartmentLine(line))
      {
        IEnumerable<ValidationFailure> departmentErrors = TryParseDepartment(line, indexedName, out DepartmentInfo? parsedDepartment);
        if (departmentErrors.Any())
        {
          errors.AddRange(departmentErrors);
        }
        else if (parsedDepartment != null)
        {
          department = parsedDepartment;
        }
      }
      else if (!string.IsNullOrEmpty(line))
      {
        IEnumerable<ValidationFailure> itemErrors = TryParseItem(line, indexedName, locale, department, out ReceiptItemUnit? item);
        if (itemErrors.Any())
        {
          errors.AddRange(itemErrors);
        }
        else if (item != null)
        {
          items.Add(item);
        }
      }
    }

    if (errors.Count > 0)
    {
      throw new ValidationException(errors);
    }

    return Task.FromResult<IEnumerable<ReceiptItemUnit>>(items);
  }
  private static bool IsDepartmentLine(string line) => line.StartsWith(DepartmentFlag);

  private static IEnumerable<ValidationFailure> TryParseDepartment(string line, string propertyName, out DepartmentInfo? parsedDepartment)
  {
    parsedDepartment = null;
    List<ValidationFailure> errors = [];

    int index = line.IndexOf(DepartmentSeparator);
    if (index < 0)
    {
      errors.Add(new ValidationFailure(propertyName, "The department line does not have a valid column count (Expected=2, Actual=1).", line)
      {
        ErrorCode = "InvalidDepartmentLineColumnCount"
      });
    }
    else
    {
      NumberUnit? number = null;
      try
      {
        number = new(line[1..index]);
      }
      catch (ValidationException exception)
      {
        errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.DepartmentNumber", error.ErrorMessage, error.AttemptedValue)
        {
          ErrorCode = error.ErrorCode
        }));
      }

      DisplayNameUnit? displayName = null;
      try
      {
        displayName = new(line[(index + 1)..]);
      }
      catch (ValidationException exception)
      {
        errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.DepartmentName", error.ErrorMessage, error.AttemptedValue)
        {
          ErrorCode = error.ErrorCode
        }));
      }

      if (number != null && displayName != null)
      {
        parsedDepartment = new(number, new DepartmentUnit(displayName));
      }
    }

    return errors;
  }

  private static IEnumerable<ValidationFailure> TryParseItem(string line, string propertyName, LocaleUnit? locale, DepartmentInfo? department, out ReceiptItemUnit? item)
  {
    item = null;
    List<ValidationFailure> errors = [];

    string[] values = line.Split(ItemSeparator);
    if (values.Length == 4 || values.Length == 6)
    {
      GtinUnit? gtin = null;
      SkuUnit? sku = null;
      string gtinOrSku = values[0].Trim();
      if (long.TryParse(gtinOrSku, out _))
      {
        try
        {
          gtin = new(gtinOrSku);
        }
        catch (ValidationException exception)
        {
          errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.Gtin", error.ErrorMessage, error.AttemptedValue)
          {
            ErrorCode = error.ErrorCode
          }));
        }
      }
      else
      {
        try
        {
          sku = new(gtinOrSku);
        }
        catch (ValidationException exception)
        {
          errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.Sku", error.ErrorMessage, error.AttemptedValue)
          {
            ErrorCode = error.ErrorCode
          }));
        }
      }

      DisplayNameUnit? label = null;
      try
      {
        label = new(values[1]);
      }
      catch (ValidationException exception)
      {
        errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.Label", error.ErrorMessage, error.AttemptedValue)
        {
          ErrorCode = error.ErrorCode
        }));
      }

      FlagsUnit? flags = null;
      try
      {
        flags = FlagsUnit.TryCreate(values[2]);
      }
      catch (ValidationException exception)
      {
        errors.AddRange(exception.Errors.Select(error => new ValidationFailure($"{propertyName}.Flags", error.ErrorMessage, error.AttemptedValue)
        {
          ErrorCode = error.ErrorCode
        }));
      }

      string priceString = values[^1].Trim();
      if (!decimal.TryParse(priceString, NumberStyles.Currency, locale?.Culture, out decimal price) || price <= 0.00m)
      {
        errors.Add(new ValidationFailure($"{propertyName}.Price", "The specified value is not a valid price.", priceString)
        {
          ErrorCode = "InvalidPrice"
        });
      }

      double quantity = 1.0d;
      decimal unitPrice = price;
      if (values.Length == 6)
      {
        string quantityString = values[3].Trim();
        if (!double.TryParse(quantityString, NumberStyles.Any, locale?.Culture, out quantity) || quantity <= 0.0d)
        {
          errors.Add(new ValidationFailure($"{propertyName}.Quantity", "The specified value is not a valid quantity.", quantityString)
          {
            ErrorCode = "InvalidQuantity"
          });
        }

        string unitPriceString = values[4].Trim();
        if (!decimal.TryParse(unitPriceString, NumberStyles.Currency, locale?.Culture, out unitPrice) || unitPrice <= 0.00m)
        {
          errors.Add(new ValidationFailure($"{propertyName}.UnitPrice", "The specified value is not a valid price.", unitPriceString)
          {
            ErrorCode = "InvalidPrice"
          });
        }
      }

      if (errors.Count == 0 && (gtin != null || sku != null) && label != null)
      {
        item = new(gtin, sku, label, flags, quantity, unitPrice, price, department?.Number, department?.Department);
      }
    }
    else
    {
      errors.Add(new ValidationFailure(propertyName, $"The item line does not have a valid column count (Expected=4 or 6, Actual={values.Length}).", line)
      {
        ErrorCode = "InvalidItemLineColumnCount"
      });
    }

    return errors;
  }
}
