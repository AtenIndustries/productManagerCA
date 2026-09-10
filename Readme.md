# ProductManager.API

A demo REST API built under ASP.NET Core 10 to showcase a simple product management, with JWT authentication and distributed environment safe unique ID generation.

Access the deployed azure version [here]((https://productmanagerapi-c6ephyf6dgcvh2bj.westus3-01.azurewebsites.net/index.html))

## Features

- CRUD operations for products
- Auto-generated unique 6-digit numerical product IDs, safe for multi-instance deployments
- Stock increment/decrement endpoints
- Product search and stock-level filtering
- JWT-based authentication
- Swagger/OpenAPI documentation
- Basic CI/CD to build, test, migrate and deploy to Azure Cloud 

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/) (for running SQL Server locally)
- SQL Server client (Azure Data Studio, SQL Server, `sqlcmd`, or the `mssql` VS Code extension)

Confirm your SDK installation:

```bash
dotnet --version
```

## Environment Setup

1. **Clone the repository**

   ```bash
   git clone <repo-url>
   cd ProductManager
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Install SQL Server** (Optional step if you already have a database)

   Get SQL Server image
   Pull

   ```bash
   docker pull mcr.microsoft.com/mssql/server:2022-latest
   ```

   Create container

   ```bash
   docker run --platform -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=P@$$W0rd' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
   ```

> [!NOTE]
> If you are using arm please add `--platform linux/amd64`

Check it's running:

```bash
docker ps
```

4. **Create database**

> [!NOTE]
> You can skip this step and directly in your SQL Server Client UI create ProductManager database.

Open an interactive shell into the container using `sqlcmd`:

```bash
docker exec -it sqlserver /opt/mssql-tools18/bin/sqlcmd \
-S localhost -U sa -P 'P@$$W0rd' -C
```

Inside the `sqlcmd` prompt, create the database:

```sql
CREATE DATABASE ProductManager;
GO
EXIT
```

5. **Configure connection string**

   Update `appsettings.Development.json`

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=productManager;User Id=sa;Password=P@$$W0rd;TrustServerCertificate=True;Encrypt=True;"
     }
   }
   ```

### 3. Apply EF Core migrations

From the API project directory:

    ```bash
    dotnet tool install --global dotnet-ef   # if not already installed
    dotnet ef database update --project ProductManager.DAL --startup-project ProductManager.API
    ```

To confirm tables, enter again in the interactive shell and

    ```sql
    USE ProductManager;
    GO
    SELECT * FROM Products;
    go
    EXIT
    ```

## Running the API

```bash
dotnet run --project ProductManager.API
```

By default the API listens on:

- `https://localhost:7240`

Swagger UI is available at the root URL. Ensure you register you own user and login before you try out the API on the Swagger UI. In case you are using another tool to test the API, make sure to add the `Authorization` header with value in the format `Bearer <TOKEN_JWT>` in any `POST`, `PUT`, `DELETE` route.

## Running Tests

### Prerequisites for the test environment

No live database credentials are required for the tests. `WebApplicationFactory` is used in ProductManager.API.Tests to mock some SQL Server behaviours not available in EFCore Memory Database, such as the creation of concurrency tokens, unique constraints and some common exceptions (ex: DBConcurrencyException), to showcase some error handling behaviours.
There was an attempt to override the API authentication in the `WebApplicationFactory` with a mock authenticator without success, so for this Code Accessment, the tests on EndPoints with `Authorize` annotation, are integrated with the JWT API authentication. That authentication is handled inside the memory db.

### Run all tests

```bash
dotnet test
```

### Run a specific test projects

Run API tests (includes Behaviou Driven Development test samples)

```bash
dotnet test ProductManager.API.Tests
```

Run business layer tests

```bash
dotnet test ProductManager.BAL.Tests
```

## Project Structure

```
ProductManager/
├── ProductManager.API/             # Web API project (controllers, Program.cs, config)
├── ProductManager.BAL/             # Business logic, DTOs
├── ProductManager.DAL/             # Data access layer: EF Core DbContext, migrations, seeding, entities
├── ProductManager.API.Tests/       # Unit tests (integrated with authentication), BDD test samples,
├── ProductManager.BAL.Tests/       # Unit tests for the business layer
├── ProductManager.BAL.CommonLib/   # Interceptors, IAuditable
└── README.md
```

## Key Design Decisions

- **Product ID generation**: unique 6-digit IDs generated using SQL Server `SEQUENCE`, to ensure atomic and collision-free generation on a distributed environment, without relying on a application-level locking.

- **Concurrency management**: product updates use a `concurrencyToken` (rowversion) to prevent lost updates when multiple clients modify the same product simultaneously.

- **Authentication**: JWT Bearer tokens, with password hashing using BCrypt.

- **Dedicated BAL methods for product modication**: each controller method of type `POST`, `PUT`, `DELETE` calls only one IProductService method, to reduce the time between product search and modification, reducing the chance of racing conditions.

- **Real authorization in API Tests**: there was an attempt to implement mocked authentication for API tests, by overriding the implement JWT one on `WebApplicationFactory`, but without success and, for the context of this Code Accessment, this tests will use real authentication, i.e., when needed, a user is created in the memory database and a real JWT token is generated for authorization. This is not a very scallable solution, because its slower and makes more sense for integration tests, but for the context of this accessment this Unit tests will have the bonus of being integrated with full Authentication. No unit tests to ensure product `Id` is created with unique 6-digit IDs, because it is a database feature, not managed in the application-level, not replicable when using EFCore MemoryDB.

- **Interceptors in Tests**: the default memory database provided by EFCore does not support `Unique` constraints, `Sequence` and `ConcurrencyToken`. Other alternatives were explored, like in memory SQL Lite, but errors occurred when trying to create sequence. Instead, interceptors to replicate some of this behaviours were added. An interceptor to simulate some common errors was also added to test responses and error handling.

- **No unit tests on Product ID generation**: No unit tests to ensure product `Id` is created with unique 6-digit IDs, because it is a database feature, not managed in the application-level.
