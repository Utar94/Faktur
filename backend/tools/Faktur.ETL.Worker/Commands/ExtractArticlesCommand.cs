using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractArticlesCommand(Mapper Mapper) : IRequest<IEnumerable<Article>>;
