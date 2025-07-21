using FastEndpoints;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Extensions;
using ProductAPI.Models;

namespace ProductAPI.Endpoints.Products
{
    /// <summary>
    /// Endpoint for retrieving all products from the system
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all products in the database.
    /// No authentication is required for this read-only operation.
    /// </remarks>
    public class GetAllProductsEndpoint : EndpointWithoutRequest<List<ProductResponse>>
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<GetAllProductsEndpoint> _logger;

        public GetAllProductsEndpoint(ProductRepository repository, ILogger<GetAllProductsEndpoint> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override void Configure()
        {
            Get("/products");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Retrieve all products";
                s.Description = "Returns a list of all products in the system";
                s.ResponseExamples[200] = new List<ProductResponse>
                {
                    new() { Id = 1, Name = "Sample Product", Price = 19.99m, Description = "A sample product" }
                };
            });
            
            Tags("Products");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Retrieving all products");
                
                var products = await _repository.GetAllAsync();
                var response = products.ToResponse().ToList();

                _logger.LogInformation("Successfully retrieved {Count} products", response.Count);
                await SendAsync(response, cancellation: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products");
                await SendErrorsAsync(500, ct);
                AddError("An error occurred while retrieving products");
            }
        }
    }
} 