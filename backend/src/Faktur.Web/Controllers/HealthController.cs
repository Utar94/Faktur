using Faktur.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Route("health")]
  public class HealthController : ControllerBase
  {
    private readonly FakturDbContext context;

    public HealthController(FakturDbContext context)
    {
      this.context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
      await context.Receipts
        .AsNoTracking()
        .ToArrayAsync(cancellationToken);

      return NoContent();
    }
  }
}
