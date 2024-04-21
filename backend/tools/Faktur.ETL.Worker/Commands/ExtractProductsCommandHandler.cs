using Faktur.Contracts.Products;
using Faktur.ETL.Worker.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractProductsCommandHandler : IRequestHandler<ExtractProductsCommand, IEnumerable<Product>>
{
  private readonly LegacyContext _context;

  public ExtractProductsCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Product>> Handle(ExtractProductsCommand command, CancellationToken cancellationToken)
  {
    ProductEntity[] products = await _context.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .Where(x => !x.Deleted && !x.Article!.Deleted && !x.Store!.Deleted)
      .ToArrayAsync(cancellationToken);
    return products.Select(command.Mapper.ToProduct);
  }
}
