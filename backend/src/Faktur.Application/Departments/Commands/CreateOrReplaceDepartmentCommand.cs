using Faktur.Application.Activities;
using Faktur.Contracts.Departments;
using MediatR;

namespace Faktur.Application.Departments.Commands;

public record CreateOrReplaceDepartmentCommand(Guid StoreId, string Number, CreateOrReplaceDepartmentPayload Payload, long? Version)
  : Activity, IRequest<CreateOrReplaceDepartmentResult?>;
