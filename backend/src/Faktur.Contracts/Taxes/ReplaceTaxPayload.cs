namespace Faktur.Contracts.Taxes;

public record ReplaceTaxPayload
{
  public double Rate { get; set; }

  public string? Flags { get; set; }
}
