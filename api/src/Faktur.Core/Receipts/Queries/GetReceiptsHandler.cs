using AutoMapper;
using Faktur.Core.Models;
using Faktur.Core.Receipts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Queries
{
  internal class GetReceiptsHandler : IRequestHandler<GetReceipts, ListModel<ReceiptModel>>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;

    public GetReceiptsHandler(IDbContext dbContext, IMapper mapper)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ListModel<ReceiptModel>> Handle(GetReceipts request, CancellationToken cancellationToken)
    {
      IQueryable<Receipt> query = dbContext.Receipts
        .AsNoTracking()
        .Include(x => x.Store);

      if (request.Deleted.HasValue)
      {
        query = query.Where(x => x.Deleted == request.Deleted.Value);
      }
      if (request.Search != null)
      {
        query = query.Where(x => x.Number != null && x.Number.Contains(request.Search));
      }
      if (request.StoreId.HasValue)
      {
        query = query.Where(x => x.StoreId == request.StoreId.Value);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (request.Sort.HasValue)
      {
        query = request.Sort.Value switch
        {
          ReceiptSort.IssuedAt => request.Desc ? query.OrderByDescending(x => x.IssuedAt) : query.OrderBy(x => x.IssuedAt),
          ReceiptSort.Number => request.Desc ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
          ReceiptSort.UpdatedAt => request.Desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          _ => throw new ArgumentException($"The receipt sort \"{request.Sort}\" is not valid.", nameof(request)),
        };
      }

      query = query.ApplyPaging(request.Index, request.Count);

      Receipt[] receipts = await query.ToArrayAsync(cancellationToken);

      return new ListModel<ReceiptModel>(mapper.Map<IEnumerable<ReceiptModel>>(receipts), total);
    }
  }
}
