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
  [Route("departments")]
  public class DepartmentController : ControllerBase
  {
    private readonly FakturDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserContext userContext;

    public DepartmentController(FakturDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.userContext = userContext;
    }

    [HttpPost("/stores/{storeId}/departments")]
    public async Task<ActionResult<DepartmentModel>> CreateAsync(
      int storeId,
      [FromBody] CreateDepartmentPayload payload,
      CancellationToken cancellationToken
    )
    {
      Store store = await dbContext.Stores
        .SingleOrDefaultAsync(x => x.Id == storeId, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(storeId);

      var department = new Department(store, userContext.Id);
      dbContext.Departments.Add(department);

      DepartmentModel model = await SaveAsync(department, payload, cancellationToken);
      var uri = new Uri($"/departments/{model.Id}");

      return Created(uri, model);
    }

    [HttpGet("/stores/{storeId}/departments")]
    public async Task<ActionResult<ListModel<DepartmentModel>>> GetAsync(
      int storeId,
      bool? deleted,
      string? search,
      DepartmentSort? sort,
      bool desc,
      int? index,
      int? count,
      CancellationToken cancellationToken
    )
    {
      Store store = await dbContext.Stores
        .SingleOrDefaultAsync(x => x.Id == storeId, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(storeId);

      IQueryable<Department> query = dbContext.Departments
        .AsNoTracking()
        .Where(x => x.StoreId == store.Id);

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
          case DepartmentSort.Name:
            query = desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            break;
          case DepartmentSort.Number:
            query = desc ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number);
            break;
          case DepartmentSort.UpdatedAt:
            query = desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt);
            break;
          default:
            return BadRequest(new { code = "invalid_sort" });
        }
      }

      query = query.ApplyPaging(index, count);

      Department[] departments = await query.ToArrayAsync(cancellationToken);

      return Ok(new ListModel<DepartmentModel>(mapper.Map<IEnumerable<DepartmentModel>>(departments), total));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentModel>> GetAsync(int id, CancellationToken cancellationToken)
    {
      Department department = await dbContext.Departments
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      return Ok(mapper.Map<DepartmentModel>(department));
    }

    [HttpPatch("{id}/delete")]
    public async Task<ActionResult<DepartmentModel>> SetDeletedAsync(int id, CancellationToken cancellationToken)
    {
      Department department = await dbContext.Departments
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      department.Delete(userContext.Id);

      await dbContext.SaveChangesAsync(cancellationToken);

      return Ok(mapper.Map<DepartmentModel>(department));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentModel>> UpdateAsync(
      int id,
      [FromBody] UpdateDepartmentPayload payload,
      CancellationToken cancellationToken
    )
    {
      Department department = await dbContext.Departments
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new EntityNotFoundException<Store>(id);

      department.Update(userContext.Id);

      return Ok(await SaveAsync(department, payload, cancellationToken));
    }

    private async Task<DepartmentModel> SaveAsync(Department department, SaveDepartmentPayload payload, CancellationToken cancellationToken)
    {
      department.Description = payload.Description?.CleanTrim();
      department.Name = payload.Name.Trim();
      department.Number = payload.Number?.CleanTrim();

      await dbContext.SaveChangesAsync(cancellationToken);

      return mapper.Map<DepartmentModel>(department);
    }
  }
}
