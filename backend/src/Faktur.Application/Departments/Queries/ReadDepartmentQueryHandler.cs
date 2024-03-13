using Faktur.Contracts.Departments;
using MediatR;

namespace Faktur.Application.Departments.Queries;

internal class ReadDepartmentQueryHandler : IRequestHandler<ReadDepartmentQuery, Department?>
{
  private readonly IDepartmentQuerier _departmentQuerier;

  public ReadDepartmentQueryHandler(IDepartmentQuerier departmentQuerier)
  {
    _departmentQuerier = departmentQuerier;
  }

  public async Task<Department?> Handle(ReadDepartmentQuery query, CancellationToken cancellationToken)
  {
    return await _departmentQuerier.ReadAsync(query.StoreId, query.Number, cancellationToken);
  }
}
