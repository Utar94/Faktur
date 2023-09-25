using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Commands;

internal record UpdateDepartmentCommand(string StoreId, string Number, UpdateDepartmentPayload Payload) : IRequest<AcceptedCommand>;
