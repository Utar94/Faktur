using Faktur.Contracts.Departments;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Departments.Queries;

public record SearchDepartmentsQuery(SearchDepartmentsPayload Payload) : IRequest<SearchResults<Department>>;
