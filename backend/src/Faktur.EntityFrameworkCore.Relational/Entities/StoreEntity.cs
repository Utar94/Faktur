using Faktur.Domain.Stores.Events;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class StoreEntity : AggregateEntity
{
  public int StoreId { get; private set; }

  public BannerEntity? Banner { get; private set; }
  public int? BannerId { get; private set; }

  public string? Number { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public StoreEntity(StoreCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName.Value;
  }

  private StoreEntity() : base()
  {
  }

  public void SetBanner(BannerEntity? banner)
  {
    Banner = banner;
    BannerId = banner?.BannerId;
  }

  public void Update(StoreUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Number != null)
    {
      Number = @event.Number.Value?.Value;
    }
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
