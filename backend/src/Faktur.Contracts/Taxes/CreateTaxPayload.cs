namespace Faktur.Contracts.Taxes;

public record CreateTaxPayload
{
  public string Code { get; set; }
  public double Rate { get; set; }

  public string? Flags { get; set; }

  public CreateTaxPayload() : this(string.Empty, 0.0)
  {
  }

  public CreateTaxPayload(string code, double rate)
  {
    Code = code;
    Rate = rate;
  }
}
