using AutoMapper;
using Faktur.Core.Receipts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Queries
{
  internal class GetReceiptHandler : IRequestHandler<GetReceipt, ReceiptModel>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;

    public GetReceiptHandler(IDbContext dbContext, IMapper mapper)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ReceiptModel> Handle(GetReceipt request, CancellationToken cancellationToken)
    {
      Receipt receipt = await dbContext.Receipts
        .AsNoTracking()
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Article)
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Department)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Receipt>(request.Id);

      return mapper.Map<ReceiptModel>(receipt);
    }
  }
}
