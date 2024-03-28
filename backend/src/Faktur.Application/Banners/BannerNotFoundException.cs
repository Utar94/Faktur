using Faktur.Domain.Banners;
using Logitar.EventSourcing;

namespace Faktur.Application.Banners;

public class BannerNotFoundException : AggregateNotFoundException<BannerAggregate>
{
  protected override string ErrorMessage { get; } = "The specified banner could not be found.";

  public BannerNotFoundException(Guid bannerId, string? propertyName = null) : base(new AggregateId(bannerId), propertyName)
  {
  }
}
