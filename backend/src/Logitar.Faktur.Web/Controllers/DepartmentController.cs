using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Web.Extensions;
using Logitar.Faktur.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Controllers;

[ApiController]
[Route("stores/{storeId}/departments")]
public class DepartmentController : ControllerBase
{
  private readonly IDepartmentService departmentService;

  public DepartmentController(IDepartmentService departmentService)
  {
    this.departmentService = departmentService;
  }

  [HttpGet("{number}")]
  public async Task<ActionResult<Department>> ReadAsync(string storeId, string number, CancellationToken cancellationToken)
  {
    Department? department = await departmentService.ReadAsync(storeId, number, cancellationToken);
    return department == null ? NotFound() : Ok(department);
  }

  [HttpDelete("{number}")]
  public async Task<ActionResult<AcceptedCommand>> RemoveAsync(string storeId, string number, CancellationToken cancellationToken)
  {
    return Accepted(await departmentService.RemoveAsync(storeId, number, cancellationToken));
  }

  [HttpPut("{number}")]
  public async Task<ActionResult<AcceptedCommand>> SaveAsync(string storeId, string number,
    [FromBody] SaveDepartmentPayload payload, CancellationToken cancellationToken)
  {
    AcceptedCommand result = await departmentService.SaveAsync(storeId, number, payload, cancellationToken);
    Uri uri = new($"{Request.GetBaseUrl()}/stores/{storeId}/departments/{number}");

    return Accepted(uri, result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Department>>> SearchAsync(string storeId, [FromQuery] SearchDepartmentsQuery query, CancellationToken cancellationToken)
  {
    return Ok(await departmentService.SearchAsync(query.ToPayload(storeId), cancellationToken));
  }

  [HttpPatch("{number}")]
  public async Task<ActionResult<AcceptedCommand>> UpdateAsync(string storeId, string number,
    [FromBody] UpdateDepartmentPayload payload, CancellationToken cancellationToken)
  {
    return Accepted(await departmentService.UpdateAsync(storeId, number, payload, cancellationToken));
  }
}
