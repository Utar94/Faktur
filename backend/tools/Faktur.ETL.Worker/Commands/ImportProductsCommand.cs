using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportProductsCommand(IEnumerable<Product> Products) : IRequest<int>;
