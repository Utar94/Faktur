using Faktur.Contracts.Articles;
using Faktur.ETL.Worker.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker.Commands;

internal class ExtractArticlesCommandHandler : IRequestHandler<ExtractArticlesCommand, IEnumerable<Article>>
{
  private readonly LegacyContext _context;

  public ExtractArticlesCommandHandler(LegacyContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Article>> Handle(ExtractArticlesCommand command, CancellationToken cancellationToken)
  {
    ArticleEntity[] articles = await _context.Articles.AsNoTracking()
      .Where(x => !x.Deleted)
      .ToArrayAsync(cancellationToken);
    return articles.Select(command.Mapper.ToArticle);
  }
}
