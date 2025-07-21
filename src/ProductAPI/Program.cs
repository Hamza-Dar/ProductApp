using FastEndpoints;
using ProductAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service discovery for Aspire integration
builder.Services.AddServiceDiscovery();

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS to allow Blazor app requests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Check if running in Aspire context or standalone
if (builder.Configuration.GetConnectionString("productdb") != null)
{
    // Running with Aspire - use Aspire's database connection
    builder.AddNpgsqlDataSource("productdb");
}
else
{
    // Running standalone - use local connection string
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");    
    builder.Services.AddNpgsqlDataSource(connectionString);
}

// Add services to the container
builder.Services.AddFastEndpoints();

// Register repository with proper lifetime scope
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

// Add basic health checks
app.MapHealthChecks("/health");

// Initialize database schema on startup
using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<ProductRepository>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await repository.EnsureDatabaseCreatedAsync();
        logger.LogInformation("Database schema initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize database schema");
        throw; // Fail fast if database setup fails
    }
}

// Configure the HTTP request pipeline
app.UseCors(); // Enable CORS
app.UseHttpsRedirection();
app.UseFastEndpoints();

app.Run();