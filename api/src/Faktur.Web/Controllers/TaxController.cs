using Faktur.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Web.Controllers
{
  [ApiController]
  [Route("taxes")]
  public class TaxController : ControllerBase
  {
    private readonly TaxesSettings taxesSettings;

    public TaxController(TaxesSettings taxesSettings)
    {
      this.taxesSettings = taxesSettings;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TaxSettings>> Get() => Ok(new[]
    {
      taxesSettings.Gst,
      taxesSettings.Qst
    });
  }
}
