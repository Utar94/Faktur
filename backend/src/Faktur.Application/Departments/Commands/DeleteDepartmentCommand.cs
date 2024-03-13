using Faktur.Application.Activities;
using Faktur.Contracts.Departments;
using MediatR;

namespace Faktur.Application.Departments.Commands;

public record DeleteDepartmentCommand(Guid StoreId, string Number) : Activity, IRequest<Department?>;
