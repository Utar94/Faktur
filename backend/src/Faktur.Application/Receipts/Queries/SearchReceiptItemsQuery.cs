using Faktur.Contracts.Receipts;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

public record SearchReceiptItemsQuery(SearchReceiptItemsPayload Payload) : IRequest<SearchResults<ReceiptItem>>;
