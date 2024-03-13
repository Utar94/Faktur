using Faktur.Contracts.Stores;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Stores.Queries;

public record SearchStoresQuery(SearchStoresPayload Payload) : IRequest<SearchResults<Store>>;
