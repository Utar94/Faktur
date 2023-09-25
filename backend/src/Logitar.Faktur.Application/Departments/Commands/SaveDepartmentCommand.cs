using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Commands;

internal record SaveDepartmentCommand(string StoreId, string Number, SaveDepartmentPayload Payload) : IRequest<AcceptedCommand>;
