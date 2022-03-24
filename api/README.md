# Faktur API

Receipt management Web API.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the **Faktur.Infrastructure** project directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to specify a name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --startup-project ../Faktur.Web`

### Remove a migration

To remove the latest new migration, execute the following command.

`dotnet ef migrations remove --startup-project ../Faktur.Web`

### Generate a script

To generate a script, execute the following command. You can optionally specify a *from* migration name.

`dotnet ef migrations script <FROM_MIGRATION_NAME>? --startup-project ../Faktur.Web`
