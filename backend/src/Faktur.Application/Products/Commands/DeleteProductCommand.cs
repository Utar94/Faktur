using Faktur.Application.Activities;
using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : Activity, IRequest<Product?>;
