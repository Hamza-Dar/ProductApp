using FastEndpoints;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Extensions;
using ProductAPI.Models;

namespace ProductAPI.Endpoints.Products
{
    /// <summary>
    /// Endpoint for updating existing products in the system
    /// </summary>
    /// <remarks>
    /// This endpoint updates an existing product with the provided information.
    /// Returns 200 OK with the updated product data.
    /// Returns 404 Not Found if the product doesn't exist.
    /// Returns 400 Bad Request if validation fails.
    /// </remarks>
    public class UpdateProductEndpoint : Endpoint<UpdateProductRequest, ProductResponse>
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<UpdateProductEndpoint> _logger;

        public UpdateProductEndpoint(ProductRepository repository, ILogger<UpdateProductEndpoint> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override void Configure()
        {
            Put("/products/{id}");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Update an existing product";
                s.Description = "Updates an existing product with the provided information";
                s.ResponseExamples[200] = new ProductResponse 
                { 
                    Id = 1, 
                    Name = "Updated Product", 
                    Price = 39.99m, 
                    Description = "Updated product description" 
                };
                s.ResponseExamples[404] = "Product not found";
                s.ResponseExamples[400] = "Validation failed";
            });
            
            Tags("Products");
        }

        public override async Task HandleAsync(UpdateProductRequest request, CancellationToken ct)
        {
            var productId = Route<int>("id");
            
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", productId);

                // Validate the request
                await ValidateRequestAsync(request);

                if (ValidationFailed)
                {
                    _logger.LogWarning("Validation failed for product update: {ProductId}", productId);
                    await SendErrorsAsync(400, ct);
                    return;
                }

                // Check if product exists
                var existingProduct = await _repository.GetByIdAsync(productId);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", productId);
                    await SendNotFoundAsync(ct);
                    return;
                }

                var product = request.ToEntity();
                var updatedProduct = await _repository.UpdateAsync(productId, product);
                
                if (updatedProduct == null)
                {
                    _logger.LogError("Failed to update product with ID: {ProductId}", productId);
                    await SendErrorsAsync(500, ct);
                    AddError("Failed to update the product");
                    return;
                }

                var response = updatedProduct.ToResponse();

                _logger.LogInformation("Successfully updated product with ID: {ProductId}", productId);
                await SendOkAsync(response, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", productId);
                await SendErrorsAsync(500, ct);
                AddError("An error occurred while updating the product");
            }
        }

        private async Task ValidateRequestAsync(UpdateProductRequest request)
        {
            // Custom business validation logic
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                AddError(r => r.Name, "Product name is required");
            }
            else if (request.Name.Length > 100)
            {
                AddError(r => r.Name, "Product name cannot exceed 100 characters");
            }

            if (request.Price <= 0)
            {
                AddError(r => r.Price, "Product price must be greater than 0");
            }

            if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 1000)
            {
                AddError(r => r.Description, "Product description cannot exceed 1000 characters");
            }

            // Simulate async validation
            await Task.CompletedTask;
        }
    }
} 