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
  [Route("stores")]
  public class StoreController : ControllerBase
  {
    private readonly FakturDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public StoreController(FakturDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.userContext = userContext;
    }

    [HttpPost]
    public async Task<ActionResult<StoreModel>> CreateAsync(
      [FromBody] CreateStorePayload payload,
      CancellationToken cancellationToken
    )
    {
      var store = new Store(userContext.Id);
      dbContext.Stores.Add(store);

      StoreModel model = await SaveAsync(store, payload, cancellationToken);
      var uri = new Uri($"/stores/{model.Id}");

      return Created(uri, model);
    }

    [HttpGet]
    public async Task<ActionResult<ListModel<StoreModel>>> GetAsync(
      int? bannerId,
      bool? deleted,
      string? search,
      StoreSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      IQueryable<Store> query = dbContext.Stores.AsNoTracking();

      if (bannerId.HasValue)
      {
        query = query.Where(x => x.BannerId == bannerId);
      }
      if (deleted.HasValue)
      {
        query = query.Where(x => x.Deleted == deleted.Value);
      }
      if (search != null)
      {
        query = query.Where(x => x.Name.Contains(search) || (x.Number != null && x.Number.Contains(search)));
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        switch (sort.Value)
        {
          case StoreSort.Name:
            query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            break;
          case StoreSort.Number:
            query = desc ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number);
            break;
          case StoreSort.UpdatedAt:
            query = desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt);
            break;
          default:
            return BadRequest(new { code = "invalid_sort" });
        }
      }

      query = query.ApplyPaging(index, count);

      Store[] stores = await query.ToArrayAsync(cancellationToken);

      return Ok(new ListModel<StoreModel>(mapper.Map<IEnumerable<StoreModel>>(stores), total));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      Store store = await dbContext.Stores
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      return Ok(mapper.Map<StoreModel>(store));
    }

    [HttpPatch("{id}/delete")]
    public async Task<ActionResult<StoreModel>> SetDeletedAsync(int id, CancellationToken cancellationToken)
    {
      Store store = await dbContext.Stores
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      store.Delete(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return Ok(mapper.Map<StoreModel>(store));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StoreModel>> UpdateAsync(
      int id,
      [FromBody] UpdateStorePayload payload,
      CancellationToken cancellationToken
    )
    {
      Store store = await dbContext.Stores
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      store.Update(userContext.Id);

      return Ok(await SaveAsync(store, payload, cancellationToken));
    }

    private async Task<StoreModel> SaveAsync(Store store, SaveStorePayload payload, CancellationToken cancellationToken)
    {
      Banner? banner = payload.BannerId.HasValue
        ? banner = await dbContext.Banners
          .SingleOrDefaultAsync(x => x.Id == payload.BannerId.Value, cancellationToken)
          ?? throw new EntityNotFoundException<Banner>(payload.BannerId.Value, nameof(payload.BannerId))
        : null;

      store.Banner = banner;
      store.BannerId = banner?.Id;
      store.Description = payload.Description?.CleanTrim();
      store.Name = payload.Name.Trim();
      store.Number = payload.Number?.CleanTrim();

      store.Address = payload.Address?.CleanTrim();
      store.City = payload.City?.CleanTrim();
      store.Country = payload.Country?.CleanTrim();
      store.Phone = payload.Phone?.CleanTrim();
      store.PostalCode = payload.PostalCode?.CleanTrim();
      store.State = payload.State?.CleanTrim();

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<StoreModel>(store);
    }
  }
}
