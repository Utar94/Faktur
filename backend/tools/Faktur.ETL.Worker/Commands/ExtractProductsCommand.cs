using Faktur.Contracts.Products;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractProductsCommand(Mapper Mapper) : IRequest<IEnumerable<Product>>;
