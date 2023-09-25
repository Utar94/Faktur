using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Application.Departments.Commands;

internal record RemoveDepartmentCommand(string StoreId, string Number) : IRequest<AcceptedCommand>;
