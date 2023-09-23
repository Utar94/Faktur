using System.ComponentModel.DataAnnotations;

namespace Faktur.Web.Settings
{
  public class ApplicationSettings
  {
    [Required]
    [Url]
    public string BaseUrl { get; set; } = null!;
  }
}
