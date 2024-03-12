using Logitar;

namespace Faktur.Infrastructure;

public class DatabaseProviderNotSupportedException : Exception
{
  private const string ErrorMessage = "The specified database provider is not supported.";

  public DatabaseProvider DatabaseProvider
  {
    get => (DatabaseProvider)Data[nameof(DatabaseProvider)]!;
    private set => Data[nameof(DatabaseProvider)] = value;
  }

  public DatabaseProviderNotSupportedException(DatabaseProvider databaseProvider) : base(BuildMessage(databaseProvider))
  {
    DatabaseProvider = databaseProvider;
  }

  private static string BuildMessage(DatabaseProvider databaseProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(DatabaseProvider), databaseProvider)
    .Build();
}
