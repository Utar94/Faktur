using Logitar.Faktur.Contracts.Departments;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Queries;

internal record ReadDepartmentQuery(string StoreId, string Number) : IRequest<Department?>;
