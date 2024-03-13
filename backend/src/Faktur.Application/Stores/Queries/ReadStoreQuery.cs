using Faktur.Contracts.Stores;
using MediatR;

namespace Faktur.Application.Stores.Queries;

public record ReadStoreQuery(Guid Id) : IRequest<Store?>;
