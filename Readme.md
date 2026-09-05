//Migration notes

//On the root

//Create migration
dotnet ef migrations add Initial --project ProductManager.DAL --startup-project ProductManager.API

//Commit migration
dotnet ef  database update --project ProductManager.DAL --startup-project ProductManager.API