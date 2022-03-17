﻿using AutoMapper;
using Faktur.Core.Receipts.Models;
using Logitar;
using Logitar.Identity.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Core.Receipts.Commands
{
  internal class UpdateReceiptHandler : IRequestHandler<UpdateReceipt, ReceiptModel>
  {
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public UpdateReceiptHandler(
      IDbContext dbContext,
      IMapper mapper,
      IUserContext userContext
    )
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<ReceiptModel> Handle(UpdateReceipt request, CancellationToken cancellationToken)
    {
      Receipt receipt = await dbContext.Receipts
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Article)
        .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x!.Department)
        .Include(x => x.Taxes)
        .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Receipt>(request.Id);

      receipt.IssuedAt = request.Payload.IssuedAt;
      receipt.Number = request.Payload.Number?.CleanTrim();

      receipt.Update(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ReceiptModel>(receipt);
    }
  }
}
