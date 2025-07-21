using FastEndpoints;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Extensions;

namespace ProductAPI.Endpoints.Products
{
    /// <summary>
    /// Endpoint for retrieving a single product by its unique identifier
    /// </summary>
    /// <remarks>
    /// This endpoint returns a specific product based on the provided ID.
    /// Returns 404 if the product is not found.
    /// </remarks>
    public class GetProductByIdEndpoint : Endpoint<GetProductByIdRequest, ProductResponse>
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<GetProductByIdEndpoint> _logger;

        public GetProductByIdEndpoint(ProductRepository repository, ILogger<GetProductByIdEndpoint> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override void Configure()
        {
            Get("/products/{id}");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Retrieve a product by ID";
                s.Description = "Returns a specific product based on the provided unique identifier";
                s.ResponseExamples[200] = new ProductResponse 
                { 
                    Id = 1, 
                    Name = "Sample Product", 
                    Price = 19.99m, 
                    Description = "A sample product" 
                };
                s.ResponseExamples[404] = "Product not found";
            });
            
            Tags("Products");
        }

        public override async Task HandleAsync(GetProductByIdRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Retrieving product with ID: {ProductId}", request.Id);
                
                var product = await _repository.GetByIdAsync(request.Id);
                
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
                    await SendNotFoundAsync(ct);
                    return;
                }

                var response = product.ToResponse();

                _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", request.Id);
                await SendOkAsync(response, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID: {ProductId}", request.Id);
                await SendErrorsAsync(500, ct);
                AddError("An error occurred while retrieving the product");
            }
        }
    }
} 