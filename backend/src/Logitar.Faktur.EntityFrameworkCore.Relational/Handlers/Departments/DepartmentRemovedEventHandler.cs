using Logitar.Faktur.Domain.Departments.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Departments;

internal class DepartmentRemovedEventHandler : INotificationHandler<DepartmentRemovedEvent>
{
  private readonly FakturContext context;

  public DepartmentRemovedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(DepartmentRemovedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity store = await context.Stores
      .Include(x => x.Departments)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<StoreEntity>(@event.AggregateId);

    DepartmentEntity? department = store.Departments.SingleOrDefault(d => d.NumberNormalized == int.Parse(@event.Number));
    if (department != null)
    {
      store.Departments.Remove(department);
    }

    await context.SaveChangesAsync(cancellationToken);
  }
}
