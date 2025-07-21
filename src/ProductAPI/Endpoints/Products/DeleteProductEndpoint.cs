using FastEndpoints;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Extensions;

namespace ProductAPI.Endpoints.Products
{
    /// <summary>
    /// Endpoint for deleting products from the system
    /// </summary>
    /// <remarks>
    /// This endpoint deletes an existing product from the system.
    /// Returns 204 No Content if the product is successfully deleted.
    /// Returns 404 Not Found if the product doesn't exist.
    /// </remarks>
    public class DeleteProductEndpoint : Endpoint<GetProductByIdRequest>
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<DeleteProductEndpoint> _logger;

        public DeleteProductEndpoint(ProductRepository repository, ILogger<DeleteProductEndpoint> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override void Configure()
        {
            Delete("/products/{id}");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Delete a product";
                s.Description = "Deletes an existing product from the system";
                s.ResponseExamples[204] = "Product successfully deleted";
                s.ResponseExamples[404] = "Product not found";
            });
            
            Tags("Products");
        }

        public override async Task HandleAsync(GetProductByIdRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID: {ProductId}", request.Id);

                // Check if product exists before attempting to delete
                var existingProduct = await _repository.GetByIdAsync(request.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", request.Id);
                    await SendNotFoundAsync(ct);
                    return;
                }

                // Additional business logic can be added here
                // For example: checking if product can be deleted (not referenced in orders, etc.)
                await ValidateProductCanBeDeletedAsync(request.Id);

                if (ValidationFailed)
                {
                    await SendErrorsAsync(400, ct);
                    return;
                }

                var deleted = await _repository.DeleteAsync(request.Id);
                
                if (deleted)
                {
                    _logger.LogInformation("Successfully deleted product with ID: {ProductId}", request.Id);
                    await SendNoContentAsync(ct);
                }
                else
                {
                    _logger.LogError("Failed to delete product with ID: {ProductId}", request.Id);
                    await SendErrorsAsync(500, ct);
                    AddError("Failed to delete the product");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", request.Id);
                await SendErrorsAsync(500, ct);
                AddError("An error occurred while deleting the product");
            }
        }

        private async Task ValidateProductCanBeDeletedAsync(int productId)
        {
            // Business logic to determine if a product can be deleted
            // For example: check if product is referenced in active orders, etc.
            
            _logger.LogDebug("Validating if product {ProductId} can be deleted", productId);
            
            // Simulate async business validation
            await Task.CompletedTask;
            
           
        }

    }
} 