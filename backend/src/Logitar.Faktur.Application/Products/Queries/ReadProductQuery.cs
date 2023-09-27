using Logitar.Faktur.Contracts.Products;
using MediatR;

namespace Logitar.Faktur.Application.Products.Queries;

internal record ReadProductQuery(string StoreId, string ArticleId) : IRequest<Product?>;
