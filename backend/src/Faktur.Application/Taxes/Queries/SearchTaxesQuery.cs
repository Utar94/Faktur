using Faktur.Contracts.Taxes;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Taxes.Queries;

public record SearchTaxesQuery(SearchTaxesPayload Payload) : IRequest<SearchResults<Tax>>;
