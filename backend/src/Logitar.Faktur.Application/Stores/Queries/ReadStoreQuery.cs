using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Queries;

internal record ReadStoreQuery(string Id) : IRequest<Store?>;
