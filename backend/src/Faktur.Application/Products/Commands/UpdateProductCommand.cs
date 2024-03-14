using Faktur.Application.Activities;
using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

public record UpdateProductCommand(Guid StoreId, Guid ArticleId, UpdateProductPayload Payload) : Activity, IRequest<Product?>;
