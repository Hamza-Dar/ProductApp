using FastEndpoints;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Extensions;
using ProductAPI.Models;

namespace ProductAPI.Endpoints.Products
{
    /// <summary>
    /// Endpoint for creating new products in the system
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new product with the provided information.
    /// Returns 201 Created with the created product data and location header.
    /// Returns 400 Bad Request if validation fails.
    /// </remarks>
    public class CreateProductEndpoint : Endpoint<CreateProductRequest, ProductResponse>
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<CreateProductEndpoint> _logger;

        public CreateProductEndpoint(ProductRepository repository, ILogger<CreateProductEndpoint> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/products");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Create a new product";
                s.Description = "Creates a new product with the provided information";
                s.ResponseExamples[201] = new ProductResponse 
                { 
                    Id = 1, 
                    Name = "New Product", 
                    Price = 29.99m, 
                    Description = "A new product description" 
                };
                s.ResponseExamples[400] = "Validation failed";
            });
            
            Tags("Products");
        }

        public override async Task HandleAsync(CreateProductRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", request.Name);

                // Additional business validation can be added here
                await ValidateRequestAsync(request);

                if (ValidationFailed)
                {
                    _logger.LogWarning("Validation failed for product creation: {ProductName}", request.Name);
                    await SendErrorsAsync(400, ct);
                    return;
                }

                var product = request.ToEntity();
                var createdProduct = await _repository.CreateAsync(product);
                var response = createdProduct.ToResponse();

                _logger.LogInformation("Successfully created product with ID: {ProductId}", createdProduct.Id);

                await SendCreatedAtAsync<GetProductByIdEndpoint>(
                    new { id = createdProduct.Id }, 
                    response, 
                    cancellation: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
                await SendErrorsAsync(500, ct);
                AddError("An error occurred while creating the product");
            }
        }

        private async Task ValidateRequestAsync(CreateProductRequest request)
        {
            // Additional custom business validation logic can be implemented here
            // For example: checking for duplicate product names, validating against business rules, etc.
            
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

            await Task.CompletedTask;
        }
    }
} 