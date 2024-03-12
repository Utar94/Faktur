using Faktur.Domain.Banners.Events;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class BannerEntity : AggregateEntity
{
  public int BannerId { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public BannerEntity(BannerCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName.Value;
  }

  private BannerEntity() : base()
  {
  }

  public void Update(BannerUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
