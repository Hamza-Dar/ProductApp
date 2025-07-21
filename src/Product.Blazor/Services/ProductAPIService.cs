using System.Net;
using System.Text.Json;

namespace Product.Blazor.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _http;
        private readonly ILogger<ProductApiService> _logger;

        public ProductApiService(HttpClient http, ILogger<ProductApiService> logger)
        {
            _http = http;
            _logger = logger;
        }

        /// <summary>
        /// Get all products from the API
        /// </summary>
        public async Task<ServiceResult<List<Models.Product>>> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all products from API");
                _logger.LogInformation("HttpClient BaseAddress: {BaseAddress}", _http.BaseAddress);
                _logger.LogInformation("About to call: {FullUrl}", $"{_http.BaseAddress}products");
                
                var response = await _http.GetAsync("products");
                
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Models.Product>>() ?? new();
                    _logger.LogInformation("Successfully fetched {Count} products", products.Count);
                    return ServiceResult<List<Models.Product>>.Success(products);
                }
                
                var error = await GetErrorMessageAsync(response);
                _logger.LogWarning("Failed to fetch products: {Error}", error);
                return ServiceResult<List<Models.Product>>.Error(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching products. BaseAddress: {BaseAddress}", _http.BaseAddress);
                return ServiceResult<List<Models.Product>>.Error("Unable to connect to the product service. Please try again later.");
            }
        }

        /// <summary>
        /// Get a single product by ID
        /// </summary>
        public async Task<ServiceResult<Models.Product?>> GetProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id);
                
                var response = await _http.GetAsync($"products/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<Models.Product>();
                    _logger.LogInformation("Successfully fetched product: {ProductName}", product?.Name ?? "Unknown");
                    return ServiceResult<Models.Product?>.Success(product);
                }
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return ServiceResult<Models.Product?>.Error($"Product with ID {id} was not found.");
                }
                
                var error = await GetErrorMessageAsync(response);
                _logger.LogWarning("Failed to fetch product {ProductId}: {Error}", id, error);
                return ServiceResult<Models.Product?>.Error(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching product {ProductId}", id);
                return ServiceResult<Models.Product?>.Error("Unable to connect to the product service. Please try again later.");
            }
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        public async Task<ServiceResult<Models.Product>> CreateProductAsync(Models.Product product)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", product.Name);
                
                var response = await _http.PostAsJsonAsync("products", product);
                
                if (response.IsSuccessStatusCode)
                {
                    var createdProduct = await response.Content.ReadFromJsonAsync<Models.Product>();
                    _logger.LogInformation("Successfully created product with ID: {ProductId}", createdProduct?.Id);
                    return ServiceResult<Models.Product>.Success(createdProduct!);
                }
                
                var error = await GetErrorMessageAsync(response);
                _logger.LogWarning("Failed to create product {ProductName}: {Error}", product.Name, error);
                return ServiceResult<Models.Product>.Error(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating product {ProductName}", product.Name);
                return ServiceResult<Models.Product>.Error("Unable to connect to the product service. Please try again later.");
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        public async Task<ServiceResult<Models.Product>> UpdateProductAsync(Models.Product product)
        {
            try
            {
                _logger.LogInformation("Updating product: {ProductId} - {ProductName}", product.Id, product.Name);
                
                var response = await _http.PutAsJsonAsync($"products/{product.Id}", product);
                
                if (response.IsSuccessStatusCode)
                {
                    var updatedProduct = await response.Content.ReadFromJsonAsync<Models.Product>();
                    _logger.LogInformation("Successfully updated product: {ProductId}", product.Id);
                    return ServiceResult<Models.Product>.Success(updatedProduct!);
                }
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", product.Id);
                    return ServiceResult<Models.Product>.Error($"Product with ID {product.Id} was not found.");
                }
                
                var error = await GetErrorMessageAsync(response);
                _logger.LogWarning("Failed to update product {ProductId}: {Error}", product.Id, error);
                return ServiceResult<Models.Product>.Error(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating product {ProductId}", product.Id);
                return ServiceResult<Models.Product>.Error("Unable to connect to the product service. Please try again later.");
            }
        }

        /// <summary>
        /// Delete a product by ID
        /// </summary>
        public async Task<ServiceResult<bool>> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                
                var response = await _http.DeleteAsync($"products/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully deleted product: {ProductId}", id);
                    return ServiceResult<bool>.Success(true);
                }
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return ServiceResult<bool>.Error($"Product with ID {id} was not found.");
                }
                
                var error = await GetErrorMessageAsync(response);
                _logger.LogWarning("Failed to delete product {ProductId}: {Error}", id, error);
                return ServiceResult<bool>.Error(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting product {ProductId}", id);
                return ServiceResult<bool>.Error("Unable to connect to the product service. Please try again later.");
            }
        }

        private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                    return $"Request failed with status {response.StatusCode}";
                
                // Try to parse FastEndpoints error response
                using var document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("errors", out var errorsElement))
                {
                    var errors = new List<string>();
                    foreach (var error in errorsElement.EnumerateArray())
                    {
                        if (error.TryGetProperty("message", out var message))
                        {
                            errors.Add(message.GetString() ?? "Unknown error");
                        }
                    }
                    return string.Join(", ", errors);
                }
                
                return content;
            }
            catch
            {
                return $"Request failed with status {response.StatusCode}";
            }
        }
    }

    /// <summary>
    /// Service result wrapper for API operations
    /// </summary>
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }

        private ServiceResult(bool isSuccess, T? data, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult<T> Success(T data) => new(true, data, null);
        public static ServiceResult<T> Error(string errorMessage) => new(false, default, errorMessage);
    }
}
