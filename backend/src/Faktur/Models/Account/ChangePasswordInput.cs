namespace Faktur.Models.Account;

public record ChangePasswordInput
{
  public string Current { get; set; }
  public string New { get; set; }

  public ChangePasswordInput() : this(string.Empty, string.Empty)
  {
  }

  public ChangePasswordInput(string current, string @new)
  {
    Current = current;
    New = @new;
  }
}
