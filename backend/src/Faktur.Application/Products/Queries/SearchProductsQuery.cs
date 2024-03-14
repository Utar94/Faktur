using Faktur.Contracts.Products;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Products.Queries;

public record SearchProductsQuery(SearchProductsPayload Payload) : IRequest<SearchResults<Product>>;
