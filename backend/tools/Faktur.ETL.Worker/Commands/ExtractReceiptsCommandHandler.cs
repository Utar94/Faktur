using Faktur.Contracts.Receipts;
using Faktur.ETL.Worker.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractReceiptsCommandHandler : IRequestHandler<ExtractReceiptsCommand, IEnumerable<Receipt>>
{
  private readonly LegacyContext _context;

  public ExtractReceiptsCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Receipt>> Handle(ExtractReceiptsCommand command, CancellationToken cancellationToken)
  {
    ReceiptEntity[] receipts = await _context.Receipts.AsNoTracking()
      .Include(x => x.Items).ThenInclude(x => x.Line)
      .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Article)
      .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Department)
      .Include(x => x.Store)
      .Include(x => x.Taxes)
      .Where(x => !x.Deleted)
      .ToArrayAsync(cancellationToken);
    return receipts.Select(command.Mapper.ToReceipt);
  }
}
