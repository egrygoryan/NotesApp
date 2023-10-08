## Notes sharing using Blazor Server.
Here's how you can run this application.

## 1. Install PostgreSQL & Entity Framework
Since we're using PostgreSQL and Entity Framework Core in this application, you have to install them first.

You could download PostgreSQL from [here]([url](https://www.postgresql.org/)) 
and maybe also pgAdmin tool for your convenience [here]([url](https://www.pgadmin.org/download/. )) 
Regarding Entity Framework, we need some tools you can install with the following command in the Package Manager Console:

`dotnet tool install --global dotnet-ef`

## 2. Update the ConnectionString
In the appsettings.json file of the project, you will find the connection string to connect to your database. 
Change it accordingly your connection.

## 3. Update the Database

`cd .\Notes`

Then you can run all the migrations of EF Core or update the database, respectively.

`dotnet ef database update`

## 4. Run the Application
And finally, you can run the app.
