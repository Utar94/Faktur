using Logitar.Faktur.Domain.Departments.Events;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers.Departments;

internal class DepartmentSavedEventHandler : INotificationHandler<DepartmentSavedEvent>
{
  private readonly FakturContext context;

  public DepartmentSavedEventHandler(FakturContext context)
  {
    this.context = context;
  }

  public async Task Handle(DepartmentSavedEvent @event, CancellationToken cancellationToken)
  {
    StoreEntity store = await context.Stores
      .Include(x => x.Departments)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<StoreEntity>(@event.AggregateId);

    DepartmentEntity? department = store.Departments.SingleOrDefault(d => d.NumberNormalized == int.Parse(@event.Number));
    if (department == null)
    {
      department = new(@event, store);
      store.Departments.Add(department);
    }
    else
    {
      department.Update(@event);
    }

    await context.SaveChangesAsync(cancellationToken);
  }
}
