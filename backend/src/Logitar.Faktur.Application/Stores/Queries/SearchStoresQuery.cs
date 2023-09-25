using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Queries;

internal record SearchStoresQuery(SearchStoresPayload Payload) : IRequest<SearchResults<Store>>;
