using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Npgsql;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.PostgreSql;

namespace Tests
{
    public class ProductApiIntegrationTests : IClassFixture<ProductApiTestFixture>
    {
        private readonly HttpClient _client;
        private readonly ProductApiTestFixture _fixture;

        public ProductApiIntegrationTests(ProductApiTestFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client;
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnInitialTestData()
        {
            // Act
            var response = await _client.GetAsync("/products");
            var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();

            // Assert
            response.EnsureSuccessStatusCode();
            products.Should().NotBeNull();
            products.Should().HaveCount(3); // We'll seed 3 initial products
            products.Should().Contain(p => p.Name == "Test Laptop");
            products.Should().Contain(p => p.Name == "Test Book");
            products.Should().Contain(p => p.Name == "Test Plant Monitor");
        }
    }

    public class ProductApiTestFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private WebApplicationFactory<Program>? _factory;

        public HttpClient Client { get; private set; } = null!;

        public ProductApiTestFixture()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpass")
                .WithCleanUp(true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            // Start the PostgreSQL container
            await _postgreSqlContainer.StartAsync();

            // Create the web application factory with the test database
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing NpgsqlDataSource registration
                        var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(NpgsqlDataSource));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Add the test database connection
                        services.AddNpgsqlDataSource(_postgreSqlContainer.GetConnectionString());
                    });
                });

            Client = _factory.CreateClient();

            // Initialize database schema and seed test data
            await InitializeTestData();
        }

        private async Task InitializeTestData()
        {
            using var scope = _factory!.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ProductRepository>();

            // Create database schema
            await repository.EnsureDatabaseCreatedAsync();

            // Seed initial test data
            var testProducts = new[]
            {
                new Product { Name = "Test Laptop", Price = 1299.99m, Description = "High-performance test laptop" },
                new Product { Name = "Test Book", Price = 49.99m, Description = "Comprehensive test guide" },
                new Product { Name = "Test Plant Monitor", Price = 79.99m, Description = "IoT test device for plants" }
            };

            foreach (var product in testProducts)
            {
                await repository.CreateAsync(product);
            }
        }

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            _factory?.Dispose();
            await _postgreSqlContainer.DisposeAsync();
        }
    }
} 