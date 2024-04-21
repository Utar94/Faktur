using Faktur.Contracts.Banners;
using Faktur.ETL.Worker.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractBannersCommandHandler : IRequestHandler<ExtractBannersCommand, IEnumerable<Banner>>
{
  private readonly LegacyContext _context;

  public ExtractBannersCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Banner>> Handle(ExtractBannersCommand command, CancellationToken cancellationToken)
  {
    BannerEntity[] banners = await _context.Banners.AsNoTracking()
      .Where(x => !x.Deleted)
      .ToArrayAsync(cancellationToken);
    return banners.Select(command.Mapper.ToBanner);
  }
}
