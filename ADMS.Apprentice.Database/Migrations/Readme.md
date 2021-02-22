# Migrations cheat sheet

The commands below can be run from the Package Manager Console window. Select the ADMS.Apprentice.Database
project as the default project before running.

## To create a new migration

    `add-migration ShortNameForMigrationHere`

## To update the Migration.sql file (do this after adding a new migration)

While we don't have too many migrations the easiest approach is to regenerate the whole thing and
overwrite the Migration.sql file. Be sure to do a diff before checking in to make sure there were no
unexpected alterations.

    `script-migration -idempotent -output "ADMS.Apprentice.Database\Migrations\Migration.sql"`

## To update the database

Running the application on your local machine will perform the migration. But if you want to do it
from the package manager instead, run this:

    `update-database`

## To revert to an older version of the database

    `update-database -migration NameOfEarlierMigration

Or if you need to generate a rollback script, use this form:

    `script-migration <from migration> <to migration>

## To deal with stuffing it up

Delete the migration files you created and undo changes to Migration.Sql and RepositoryModelSnapshot.cs.
Use the above step to revert to an older version, and then go through the process of adding your migration again.

## If package manager console is broken

Package manager periodically stops working for us because of group policy regarding powershell signing. To deal with this you will need the dotnet ef tools installed if you haven't got them already:

	`dotnet tool install --global dotnet-ef --version 3.0`

Run from a command prompt in the ADMS.Apprentice.Database directory.

    `dotnet ef migrations add ShortNameForMigrationHere`

    `dotnet ef migrations script --idempotent --output "Migrations\Migration.sql"`

    `dotnet ef database update`

    `dotnet ef database update NameOfEarlierMigration`