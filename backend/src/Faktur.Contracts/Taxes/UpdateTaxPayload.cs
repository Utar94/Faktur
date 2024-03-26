namespace Faktur.Contracts.Taxes;

public record UpdateTaxPayload
{
  public double? Rate { get; set; }

  public Modification<string>? Flags { get; set; }
}
