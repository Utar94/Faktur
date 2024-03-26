namespace Faktur.Contracts.Taxes;

public record UpdateTaxPayload
{
  public string? Code { get; set; }
  public double? Rate { get; set; }

  public Modification<string>? Flags { get; set; }
}
