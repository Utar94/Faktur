using Logitar.Faktur.Domain.Articles.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class ArticleEntity : AggregateEntity
{
  public int ArticleId { get; private set; }

  public string? Gtin { get; private set; }
  public long? GtinNormalized { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public List<ProductEntity> Products { get; private set; } = new();

  public ArticleEntity(ArticleCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName.Value;
  }

  private ArticleEntity() : base()
  {
  }

  public void Update(ArticleUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Gtin != null)
    {
      Gtin = @event.Gtin.Value?.Value;
      GtinNormalized = @event.Gtin.Value?.NormalizedValue;
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
