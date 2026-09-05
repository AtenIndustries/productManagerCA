//Migration notes

//On the root

//Create migration
dotnet ef migrations add  AddSeqForId --project ProductManager.DAL --startup-project ProductManager.API

//Commit migration
dotnet ef  database update --project ProductManager.DAL --startup-project ProductManager.API

//Remove a migration
dotnet ef migrations remove  --project ProductManager.DAL --startup-project ProductManager.API


//Mention the error handling that is needed for sequence when reaches max
//Mention that a good alternative would be using snowflake, with stuff else to create ids in a distributed database context