using Faktur.Application.Departments.Commands;
using Faktur.Application.Departments.Queries;
using Faktur.Contracts.Departments;
using Faktur.Extensions;
using Faktur.Models.Departments;
using Logitar.Portal.Contracts.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Authorize]
[Route("stores/{storeId}/departments")]
public class DepartmentController : ControllerBase
{
  private readonly IMediator _mediator;

  public DepartmentController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPut("{number}")]
  public async Task<ActionResult<Department>> CreateOrReplaceAsync(Guid storeId, string number, [FromBody] CreateOrReplaceDepartmentPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceDepartmentResult? result = await _mediator.Send(new CreateOrReplaceDepartmentCommand(storeId, number, payload, version), cancellationToken);
    if (result == null)
    {
      return NotFound();
    }
    else if (result.IsCreated)
    {
      Department department = result.Department;
      Uri location = HttpContext.BuildLocation("stores/{storeId}/departments/{number}", new Dictionary<string, string>
      {
        ["storeId"] = department.Store.Id.ToString(),
        ["number"] = department.Number
      });
      return Created(location, department);
    }
    else
    {
      return Ok(result.Department);
    }
  }

  [HttpDelete("{number}")]
  public async Task<ActionResult<Department>> DeleteAsync(Guid storeId, string number, CancellationToken cancellationToken)
  {
    Department? department = await _mediator.Send(new DeleteDepartmentCommand(storeId, number), cancellationToken);
    return department == null ? NotFound() : Ok(department);
  }

  [HttpGet("{number}")]
  public async Task<ActionResult<Department>> ReadAsync(Guid storeId, string number, CancellationToken cancellationToken)
  {
    Department? department = await _mediator.Send(new ReadDepartmentQuery(storeId, number), cancellationToken);
    return department == null ? NotFound() : Ok(department);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Department>>> SearchAsync(Guid storeId, [FromQuery] SearchDepartmentsModel model, CancellationToken cancellationToken)
  {
    SearchResults<Department> results = await _mediator.Send(new SearchDepartmentsQuery(model.ToPayload(storeId)), cancellationToken);
    return Ok(results);
  }

  [HttpPatch("{number}")]
  public async Task<ActionResult<Department>> UpdateAsync(Guid storeId, string number, [FromBody] UpdateDepartmentPayload payload, CancellationToken cancellationToken)
  {
    Department? department = await _mediator.Send(new UpdateDepartmentCommand(storeId, number, payload), cancellationToken);
    return department == null ? NotFound() : Ok(department);
  }
}
