using System.Globalization;

namespace Faktur.Core.Receipts.Parsing
{
  public interface IReceiptParser
  {
    IEnumerable<LineInfo> Execute(CultureInfo culture, string? lines);
  }
}
