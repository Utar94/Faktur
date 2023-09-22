# Faktur

Receipt management Web application.

## Dependencies

To run the Faktur application, you need a running database. Supported database providers are listed
in the `DatabaseProvider` enumeration.

The default database provider is `EntityFrameworkCorePostgreSQL`. You can override it by adding it
to your user secrets. Right-click the `Logitar.Faktur` project, then click `Manage User Secrets`.
You can copy the variable in the `secrets.example.json` file and replace the `DatabaseProvider` key
by your desired database provider enumeration value.

You can use a Docker container by executing one of the following commands. The connection strings
are already configured in the `appsettings.Development.json` file.

### PostgreSQL

`docker run --name Logitar.Faktur_postgres -e POSTGRES_PASSWORD=P@s$W0rD -p 5434:5432 -d postgres`

### SQL Server

`docker run --name Logitar.Faktur_sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@s$W0rD" -p 1434:1433 -d mcr.microsoft.com/mssql/server:2022-latest`
