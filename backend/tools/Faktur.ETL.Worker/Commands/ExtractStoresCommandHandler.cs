using Faktur.Contracts.Stores;
using Faktur.ETL.Worker.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractStoresCommandHandler : IRequestHandler<ExtractStoresCommand, IEnumerable<Store>>
{
  private readonly LegacyContext _context;

  public ExtractStoresCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Store>> Handle(ExtractStoresCommand command, CancellationToken cancellationToken)
  {
    StoreEntity[] stores = await _context.Stores.AsNoTracking()
      .Include(x => x.Banner)
      .Include(x => x.Departments)
      .Where(x => !x.Deleted)
      .ToArrayAsync(cancellationToken);
    return stores.Select(command.Mapper.ToStore);
  }
}
