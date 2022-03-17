using Faktur.Core.Receipts.Payloads;
using Logitar.Validation;
using Logitar.WebApiToolKit.Core.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Faktur.Core.Receipts.Parsing
{
  public class TsvReceiptParser : IReceiptParser
  {
    public IEnumerable<LineInfo> Execute(CultureInfo culture, string? lines)
    {
      ArgumentNullException.ThrowIfNull(culture);
      ArgumentNullException.ThrowIfNull(lines);

      DepartmentInfo? department = null;

      string[] linesArray = lines.Split('\n');

      var linesData = new List<LineInfo>(capacity: linesArray.Length);
      var result = new CompositeResult(
        errorMessage: "The receipt lines have one or many errors.",
        memberNames: new[] { nameof(ImportReceiptPayload.Lines) }
      );

      for (int i = 0; i < linesArray.Length; i++)
      {
        string line = linesArray[i];
        var memberNames = new[] { $"{nameof(ImportReceiptPayload.Lines)}[{i}]" };

        if (line.StartsWith('*'))
        {
          int position = line.IndexOf('-');
          if (position >= 0)
          {
            department = new DepartmentInfo
            {
              Name = line[(position + 1)..].Trim(),
              Number = line[1..position].Trim()
            };

            if (string.IsNullOrEmpty(department.Name))
            {
              result.Add(new ValidationResult("The department name is required.", memberNames));
            }
            if (string.IsNullOrEmpty(department.Number))
            {
              result.Add(new ValidationResult("The department number is required.", memberNames));
            }
          }
          else
          {
            result.Add(new ValidationResult($"The department line is not valid. Line={line}", memberNames));
          }
        }
        else
        {
          string[] values = line.Split('\t');
          if (values.Length == 4 || values.Length == 6)
          {
            var lineData = new LineInfo
            {
              Department = department,
              Flags = values[2].Trim(),
              Id = values[0].Trim(),
              Label = values[1].Trim()
            };

            if (string.IsNullOrEmpty(lineData.Id))
            {
              result.Add(new ValidationResult("The item identifier is required.", memberNames));
            }
            if (string.IsNullOrEmpty(lineData.Label))
            {
              result.Add(new ValidationResult("The item label is required.", memberNames));
            }

            string priceString = values.Last().Trim();
            if (TryParsePrice(priceString, culture, out decimal price))
            {
              lineData.Price = price;
            }
            else
            {
              result.Add(new ValidationResult($"The item price is not valid. Price={priceString}", memberNames));
            }

            if (values.Length == 6)
            {
              string quantityString = values[3].Trim();
              if (double.TryParse(quantityString, NumberStyles.Any, culture, out double quantity))
              {
                lineData.Quantity = quantity;
              }
              else
              {
                result.Add(new ValidationResult($"The item quantity is not valid. Quantity={quantityString}", memberNames));
              }

              string unitPriceString = values[4].Trim();
              if (TryParsePrice(unitPriceString, culture, out decimal unitPrice))
              {
                lineData.UnitPrice = unitPrice;
              }
              else
              {
                result.Add(new ValidationResult($"The item unit price is not valid. UnitPrice={unitPriceString}", memberNames));
              }
            }

            linesData.Add(lineData);
          }
          else
          {
            result.Add(new ValidationResult($"The item line doesn't have a valid column count (Count={values.Length}).", memberNames));
          }
        }
      }

      if (result.Results.Any())
      {
        throw new BadRequestException(result);
      }

      return linesData;
    }

    private static bool TryParsePrice(string s, CultureInfo culture, out decimal price)
    {
      return decimal.TryParse(s, NumberStyles.Currency, culture, out price);
    }
  }
}
