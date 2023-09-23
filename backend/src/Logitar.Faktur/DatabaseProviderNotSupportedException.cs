namespace Logitar.Faktur;

public class DatabaseProviderNotSupportedException : NotSupportedException
{
  public DatabaseProviderNotSupportedException(DatabaseProvider databaseProvider)
    : base($"The database provider '{databaseProvider}' is not supported.")
  {
    Data["DatabaseProvider"] = databaseProvider;
  }
}
