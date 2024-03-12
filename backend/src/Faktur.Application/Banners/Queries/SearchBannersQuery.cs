using Faktur.Contracts.Banners;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Banners.Queries;

public record SearchBannersQuery(SearchBannersPayload Payload) : IRequest<SearchResults<Banner>>;
