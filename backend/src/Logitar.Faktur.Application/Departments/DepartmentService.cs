using Logitar.Faktur.Application.Departments.Commands;
using Logitar.Faktur.Application.Departments.Queries;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Departments;

internal class DepartmentService : IDepartmentService
{
  private readonly IMediator mediator;

  public DepartmentService(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public async Task<Department?> ReadAsync(string storeId, string number, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReadDepartmentQuery(storeId, number), cancellationToken);
  }

  public async Task<AcceptedCommand> RemoveAsync(string storeId, string number, CancellationToken cancellationToken)
  {
    return await mediator.Send(new RemoveDepartmentCommand(storeId, number), cancellationToken);
  }

  public async Task<AcceptedCommand> SaveAsync(string storeId, string number, SaveDepartmentPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SaveDepartmentCommand(storeId, number, payload), cancellationToken);
  }

  public async Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SearchDepartmentsQuery(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> UpdateAsync(string storeId, string number, UpdateDepartmentPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new UpdateDepartmentCommand(storeId, number, payload), cancellationToken);
  }
}
