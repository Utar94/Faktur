# Logitar.Faktur.EntityFrameworkCore.SqlServer

Provides an implementation of a relational event store to be used with Faktur receipt management system, Entity Framework Core and Microsoft SQL Server.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --context FakturContext --project src/Logitar.Faktur.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Faktur`

### Remove a migration

To remove the latest unapplied migration, execute the following command.

`dotnet ef migrations remove --context FakturContext --project src/Logitar.Faktur.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Faktur`

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

`dotnet ef migrations script <SOURCE_MIGRATION> --context FakturContext --project src/Logitar.Faktur.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Faktur`
