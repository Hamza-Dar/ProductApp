using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL using AddContainer for more direct control
var postgres = builder.AddContainer("postgres", "postgres", "15-alpine")
    .WithEnvironment("POSTGRES_DB", "productdb")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "Password123")
    .WithEndpoint(5432, targetPort: 5432, name: "postgres");

// Add the ProductAPI service with database reference and proper configuration
var productApi = builder.AddProject<Projects.ProductAPI>("productapi")
    .WithEnvironment("ConnectionStrings__productdb", "Host=localhost;Port=5432;Database=productdb;Username=postgres;Password=Password123")
    .WaitFor(postgres);

// Add the Blazor frontend with reference to the API
var blazorApp = builder.AddProject<Projects.Product_Blazor>("product-blazor")
    .WithReference(productApi)
    .WaitFor(productApi);

builder.Build().Run();


//https://raw.githubusercontent.com/github/gitignore/main/VisualStudio.gitignore