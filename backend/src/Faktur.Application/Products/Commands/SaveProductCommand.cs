using Faktur.Domain.Products;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal record SaveProductCommand(ProductAggregate Product) : INotification;
