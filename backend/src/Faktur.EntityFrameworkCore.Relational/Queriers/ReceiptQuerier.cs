using Faktur.Application.Receipts;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ReceiptQuerier : IReceiptQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ReceiptEntity> _receipts;

  public ReceiptQuerier(IActorService actorService, FakturContext context)
  {
    _actorService = actorService;
    _receipts = context.Receipts;
  }

  public async Task<Receipt> ReadAsync(ReceiptAggregate receipt, CancellationToken cancellationToken)
  {
    return await ReadAsync(receipt.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The receipt 'AggregateId={receipt.Id.Value}' could not be found.");
  }
  public async Task<Receipt?> ReadAsync(ReceiptId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Receipt?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ReceiptEntity? receipt = await _receipts.AsNoTracking()
      .Include(x => x.Items)
      .Include(x => x.Taxes)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken); // TODO(fpion): other includes

    return receipt == null ? null : await MapAsync(receipt, cancellationToken);
  }

  private async Task<Receipt> MapAsync(ReceiptEntity receipt, CancellationToken cancellationToken)
  {
    return (await MapAsync([receipt], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Receipt>> MapAsync(IEnumerable<ReceiptEntity> receipts, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = receipts.SelectMany(receipt => receipt.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return receipts.Select(mapper.ToReceipt);
  }
}
