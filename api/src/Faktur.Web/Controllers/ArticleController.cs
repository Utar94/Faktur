using AutoMapper;
using Faktur.Core;
using Faktur.Core.Articles;
using Faktur.Core.Articles.Models;
using Faktur.Core.Articles.Payloads;
using Faktur.Core.Models;
using Faktur.Core.Stores;
using Faktur.Infrastructure;
using Logitar;
using Logitar.Identity.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Web.Controllers
{
  [ApiController]
  [Authorize]
  [Route("articles")]
  public class ArticleController : ControllerBase
  {
    private readonly FakturDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public ArticleController(FakturDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.userContext = userContext;
    }

    [HttpPost]
    public async Task<ActionResult<ArticleModel>> CreateAsync(
      [FromBody] CreateArticlePayload payload,
      CancellationToken cancellationToken
    )
    {
      var article = new Article(userContext.Id);
      dbContext.Articles.Add(article);

      ArticleModel model = await SaveAsync(article, payload, cancellationToken);
      var uri = new Uri($"/articles/{model.Id}");

      return Created(uri, model);
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<ArticleModel>>> GetAsync(
      bool? deleted,
      string? search,
      ArticleSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      IQueryable<Article> query = dbContext.Articles.AsNoTracking();

      if (deleted.HasValue)
      {
        query = query.Where(x => x.Deleted == deleted.Value);
      }
      if (search != null)
      {
        query = query.Where(x => x.Name.Contains(search));
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        switch (sort.Value)
        {
          case ArticleSort.Name:
            query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            break;
          case ArticleSort.UpdatedAt:
            query = desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt);
            break;
          default:
            return BadRequest(new { code = "invalid_sort" });
        }
      }

      query = query.ApplyPaging(index, count);

      Article[] articles = await query.ToArrayAsync(cancellationToken);

      return Ok(new ListModel<ArticleModel>(mapper.Map<IEnumerable<ArticleModel>>(articles), total));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      Article article = await dbContext.Articles
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      return Ok(mapper.Map<ArticleModel>(article));
    }

    [HttpPatch("{id}/delete")]
    public async Task<ActionResult<ArticleModel>> SetDeletedAsync(int id, CancellationToken cancellationToken)
    {
      Article article = await dbContext.Articles
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      article.Delete(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return Ok(mapper.Map<ArticleModel>(article));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ArticleModel>> UpdateAsync(
      int id,
      [FromBody] UpdateArticlePayload payload,
      CancellationToken cancellationToken
    )
    {
      Article article = await dbContext.Articles
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      article.Update(userContext.Id);

      return Ok(await SaveAsync(article, payload, cancellationToken));
    }

    private async Task<ArticleModel> SaveAsync(Article article, SaveArticlePayload payload, CancellationToken cancellationToken)
    {
      article.Description = payload.Description?.CleanTrim();
      article.Gtin = payload.Gtin;
      article.Name = payload.Name.Trim();

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<ArticleModel>(article);
    }
  }
}
