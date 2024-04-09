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

  public Task<IEnumerable<ReceiptItemUnit>> ExecuteAsync(string linesRaw, LocaleUnit? locale, CancellationToken cancellationToken)
  {
    NumberUnit? departmentNumber = null;
    DepartmentUnit? department = null;

    string[] lines = linesRaw.Trim().Remove("\r").Split('\n');

    int capacity = lines.Length;
    List<ReceiptItemUnit> items = new(capacity);
    List<ValidationFailure> errors = new(capacity);

    for (int i = 0; i < lines.Length; i++)
    {
      string line = lines[i].Trim();
      if (line.StartsWith(DepartmentFlag))
      {
        string[] values = line[1..].Split(DepartmentSeparator);
        if (values.Length != 2)
        {
          // TODO(fpion): validation?
        }

        departmentNumber = new(values[0]); // TODO(fpion): validation?
        department = new(new DisplayNameUnit(values[1])); // TODO(fpion): validation?
      }
      else if (!string.IsNullOrEmpty(line))
      {
        string[] values = line.Split(ItemSeparator);
        if (values.Length != 4 && values.Length != 6)
        {
          // TODO(fpion): validation?
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
        decimal price = decimal.Parse(values[^1]); // TODO(fpion): validation?

        double quantity = 1.0d;
        decimal unitPrice = price;
        if (values.Length == 6)
        {
          quantity = double.Parse(values[3]); // TODO(fpion): validation?
          unitPrice = decimal.Parse(values[4]); // TODO(fpion): validation?
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
}
