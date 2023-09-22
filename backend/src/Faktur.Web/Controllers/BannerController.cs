using AutoMapper;
using Faktur.Core;
using Faktur.Core.Models;
using Faktur.Core.Stores;
using Faktur.Core.Stores.Models;
using Faktur.Core.Stores.Payloads;
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
  [Route("banners")]
  public class BannerController : ControllerBase
  {
    private readonly FakturDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public BannerController(FakturDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.userContext = userContext;
    }

    [HttpPost]
    public async Task<ActionResult<BannerModel>> CreateAsync(
      [FromBody] CreateBannerPayload payload,
      CancellationToken cancellationToken
    )
    {
      var banner = new Banner(userContext.Id);
      dbContext.Banners.Add(banner);

      BannerModel model = await SaveAsync(banner, payload, cancellationToken);
      var uri = new Uri($"/banners/{model.Id}");

      return Created(uri, model);
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<BannerModel>>> GetAsync(
      bool? deleted,
      string? search,
      BannerSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      IQueryable<Banner> query = dbContext.Banners.AsNoTracking();

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
          case BannerSort.Name:
            query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            break;
          case BannerSort.UpdatedAt:
            query = desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt);
            break;
          default:
            return BadRequest(new { code = "invalid_sort" });
        }
      }

      query = query.ApplyPaging(index, count);

      Banner[] banners = await query.ToArrayAsync(cancellationToken);

      return Ok(new ListModel<BannerModel>(mapper.Map<IEnumerable<BannerModel>>(banners), total));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BannerModel>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
      Banner banner = await dbContext.Banners
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Banner>(id);

      dbContext.Banners.Remove(banner);
      await dbContext.SaveChangesAsync(cancellationToken);

      return Ok(mapper.Map<BannerModel>(banner));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BannerModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      Banner banner = await dbContext.Banners
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Banner>(id);

      return Ok(mapper.Map<BannerModel>(banner));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BannerModel>> UpdateAsync(
      int id,
      [FromBody] UpdateBannerPayload payload,
      CancellationToken cancellationToken
    )
    {
      Banner banner = await dbContext.Banners
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Banner>(id);

      banner.Update(userContext.Id);

      return Ok(await SaveAsync(banner, payload, cancellationToken));
    }

    private async Task<BannerModel> SaveAsync(Banner banner, SaveBannerPayload payload, CancellationToken cancellationToken)
    {
      banner.Description = payload.Description?.CleanTrim();
      banner.Name = payload.Name.Trim();

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<BannerModel>(banner);
    }
  }
}
