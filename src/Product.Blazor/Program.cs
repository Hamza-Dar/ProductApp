using Product.Blazor.Components;
using Product.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service discovery for Aspire integration
builder.Services.AddServiceDiscovery();

// Add health checks
builder.Services.AddHealthChecks();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register ProductApiService
builder.Services.AddScoped<ProductApiService>();

// Configure HttpClient for ProductAPI with proper Aspire service discovery
builder.Services.AddHttpClient<ProductApiService>(client =>
{
    // Use Aspire service name - this will be resolved dynamically by service discovery
    client.BaseAddress = new Uri("http://productapi");
    client.DefaultRequestHeaders.Add("User-Agent", "ProductManagement-BlazorApp/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddServiceDiscovery(); // Enable service discovery for this HttpClient

var app = builder.Build();

// Add basic health checks
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
