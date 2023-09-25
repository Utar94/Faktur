using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Queries;

internal record SearchDepartmentsQuery(SearchDepartmentsPayload Payload) : IRequest<SearchResults<Department>>;
