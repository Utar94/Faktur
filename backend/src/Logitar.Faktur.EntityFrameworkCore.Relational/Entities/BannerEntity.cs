using Logitar.Faktur.Domain.Banners.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class BannerEntity : AggregateEntity
{
  public int BannerId { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public List<StoreEntity> Stores { get; private set; } = new();

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
