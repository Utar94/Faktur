using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Products.Queries;

internal record SearchProductsQuery(SearchProductsPayload Payload) : IRequest<SearchResults<Product>>;
