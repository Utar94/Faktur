using Microsoft.AspNetCore.Mvc;

namespace Faktur.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Route("")]
  public class IndexController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get() => Ok("Hello World!");
  }
}
