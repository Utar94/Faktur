namespace Faktur.Contracts.Taxes;

public record ReplaceTaxPayload
{
  public string Code { get; set; }
  public double Rate { get; set; }

  public string? Flags { get; set; }

  public ReplaceTaxPayload() : this(string.Empty, 0.0)
  {
  }

  public ReplaceTaxPayload(string code, double rate)
  {
    Code = code;
    Rate = rate;
  }
}
