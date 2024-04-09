using Faktur.Domain.Receipts;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Parsing;

internal interface IReceiptParser
{
  Task<IEnumerable<ReceiptItemUnit>> ExecuteAsync(string lines, string propertyName, LocaleUnit? locale = null, CancellationToken cancellationToken = default);
}
