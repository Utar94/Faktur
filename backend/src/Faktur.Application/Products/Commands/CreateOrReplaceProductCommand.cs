using Faktur.Application.Activities;
using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

public record CreateOrReplaceProductCommand(Guid StoreId, Guid ArticleId, CreateOrReplaceProductPayload Payload, long? Version)
  : Activity, IRequest<CreateOrReplaceProductResult>;
