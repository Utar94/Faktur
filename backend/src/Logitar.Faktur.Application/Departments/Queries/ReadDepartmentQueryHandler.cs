using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Stores;
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
    StoreId storeId = StoreId.Parse(query.StoreId, nameof(query.StoreId));
    DepartmentNumberUnit number = DepartmentNumberUnit.Parse(query.Number, nameof(query.Number));

    return await departmentQuerier.ReadAsync(storeId, number, cancellationToken);
  }
}
