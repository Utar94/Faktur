using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using MediatR;

namespace Logitar.Faktur.Application.Products.Commands;

internal record SaveProductCommand(string StoreId, string ArticleId, SaveProductPayload Payload) : IRequest<AcceptedCommand>;
