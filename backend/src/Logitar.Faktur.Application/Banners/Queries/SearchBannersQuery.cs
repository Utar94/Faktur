using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Queries;

internal record SearchBannersQuery(SearchBannersPayload Payload) : IRequest<SearchResults<Banner>>;
