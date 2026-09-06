//Migration notes

//On the root

//Create migration
dotnet ef migrations add  AddProductConcurrencyToken --project ProductManager.DAL --startup-project ProductManager.API

//Commit migration
dotnet ef  database update --project ProductManager.DAL --startup-project ProductManager.API

//Remove a migration
dotnet ef migrations remove  --project ProductManager.DAL --startup-project ProductManager.API


//Mention the error handling that is needed for sequence when reaches max
//Mention that a good alternative would be using snowflake, with stuff else to create ids in a distributed database context



Install DB

#be sure the docker demon is running
docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run --platform linux/amd64  -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=P@$$W0rd' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

