namespace Faktur.Contracts.Taxes;

public record ReplaceTaxPayload
{
  public string Code { get; set; }
  public double Rate { get; set; }

  public string? Flags { get; set; }
}
