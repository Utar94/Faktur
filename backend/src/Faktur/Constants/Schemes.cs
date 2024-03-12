namespace Faktur.Constants;

internal static class Schemes
{
  public const string Basic = nameof(Basic);
  public const string Bearer = nameof(Bearer);
  public const string Session = nameof(Session);

  public static IReadOnlyCollection<string> All => [Basic, Bearer, Session];
}
