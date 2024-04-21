using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportArticlesCommand(IEnumerable<Article> Articles) : IRequest<int>;
