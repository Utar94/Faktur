using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Taxes;

public class Tax : Aggregate
{
  public string Code { get; set; }
  public double Rate { get; set; }

  public string? Flags { get; set; }

  public Tax() : this(string.Empty, 0.0)
  {
  }

  public Tax(string code, double rate)
  {
    Code = code;
    Rate = rate;
  }
}
