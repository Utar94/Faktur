using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Application.Products.Commands;

internal record RemoveProductCommand(string StoreId, string ArticleId) : IRequest<AcceptedCommand>;
