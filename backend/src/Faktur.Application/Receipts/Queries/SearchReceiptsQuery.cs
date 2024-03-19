using Faktur.Contracts.Receipts;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

public record SearchReceiptsQuery(SearchReceiptsPayload Payload) : IRequest<SearchResults<Receipt>>;
