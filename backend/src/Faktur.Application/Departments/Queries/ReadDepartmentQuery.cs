using Faktur.Contracts.Departments;
using MediatR;

namespace Faktur.Application.Departments.Queries;

public record ReadDepartmentQuery(Guid StoreId, string Number) : IRequest<Department?>;
