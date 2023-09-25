using Logitar.Faktur.Contracts.Departments;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Queries;

internal class ReadDepartmentQueryHandler : IRequestHandler<ReadDepartmentQuery, Department?>
{
  private readonly IDepartmentQuerier departmentQuerier;

  public ReadDepartmentQueryHandler(IDepartmentQuerier departmentQuerier)
  {
    this.departmentQuerier = departmentQuerier;
  }

  public async Task<Department?> Handle(ReadDepartmentQuery query, CancellationToken cancellationToken)
  {
    return await departmentQuerier.ReadAsync(query.StoreId, query.Number, cancellationToken);
  }
}
