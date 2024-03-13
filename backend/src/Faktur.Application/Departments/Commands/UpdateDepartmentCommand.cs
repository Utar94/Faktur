using Faktur.Application.Activities;
using Faktur.Contracts.Departments;
using MediatR;

namespace Faktur.Application.Departments.Commands;

public record UpdateDepartmentCommand(Guid StoreId, string Number, UpdateDepartmentPayload Payload) : Activity, IRequest<Department?>;
