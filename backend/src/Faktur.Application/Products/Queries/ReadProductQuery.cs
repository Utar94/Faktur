using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.Application.Products.Queries;

public record ReadProductQuery(Guid? Id, Guid? StoreId, Guid? ArticleId, string? Sku) : IRequest<Product?>;
