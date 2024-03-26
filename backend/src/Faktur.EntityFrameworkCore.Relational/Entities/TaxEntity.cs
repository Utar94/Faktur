using Faktur.Domain.Taxes.Events;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class TaxEntity : AggregateEntity
{
  public int TaxId { get; private set; }

  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => Code.ToUpper();
    private set { }
  }
  public double Rate { get; private set; }

  public string? Flags { get; private set; }

  public TaxEntity(TaxCreatedEvent @event) : base(@event)
  {
    Code = @event.Code.Value;
    Rate = @event.Rate;
  }

  private TaxEntity()
  {
  }

  public void Update(TaxUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Code != null)
    {
      Code = @event.Code.Value;
    }
    if (@event.Rate.HasValue)
    {
      Rate = @event.Rate.Value;
    }

    if (@event.Flags != null)
    {
      Flags = @event.Flags.Value?.Value;
    }
  }
}
