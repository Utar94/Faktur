using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using MediatR;

namespace Logitar.Faktur.Application.Products.Commands;

internal record UpdateProductCommand(string StoreId, string ArticleId, UpdateProductPayload Payload) : IRequest<AcceptedCommand>;
