using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Queries;

internal class SearchDepartmentsQueryHandler : IRequestHandler<SearchDepartmentsQuery, SearchResults<Department>>
{
  private readonly IDepartmentQuerier departmentQuerier;

  public SearchDepartmentsQueryHandler(IDepartmentQuerier departmentQuerier)
  {
    this.departmentQuerier = departmentQuerier;
  }

  public async Task<SearchResults<Department>> Handle(SearchDepartmentsQuery query, CancellationToken cancellationToken)
  {
    return await departmentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
