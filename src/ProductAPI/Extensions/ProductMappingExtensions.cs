using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Extensions
{
    /// <summary>
    /// Extension methods for mapping between Product entities and DTOs
    /// </summary>
    public static class ProductMappingExtensions
    {
        /// <summary>
        /// Converts a Product entity to a ProductResponse DTO
        /// </summary>
        /// <param name="product">The product entity to convert</param>
        /// <returns>A ProductResponse DTO</returns>
        public static ProductResponse ToResponse(this Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };
        }

        /// <summary>
        /// Converts a collection of Product entities to ProductResponse DTOs
        /// </summary>
        /// <param name="products">The collection of product entities</param>
        /// <returns>A collection of ProductResponse DTOs</returns>
        public static IEnumerable<ProductResponse> ToResponse(this IEnumerable<Product> products)
        {
            ArgumentNullException.ThrowIfNull(products);
            
            return products.Select(p => p.ToResponse());
        }

        /// <summary>
        /// Converts a CreateProductRequest DTO to a Product entity
        /// </summary>
        /// <param name="request">The create request DTO</param>
        /// <returns>A Product entity</returns>
        public static Product ToEntity(this CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            return new Product
            {
                Name = request.Name.Trim(),
                Price = request.Price,
                Description = request.Description?.Trim() ?? string.Empty
            };
        }

        /// <summary>
        /// Converts an UpdateProductRequest DTO to a Product entity
        /// </summary>
        /// <param name="request">The update request DTO</param>
        /// <returns>A Product entity</returns>
        public static Product ToEntity(this UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            return new Product
            {
                Name = request.Name.Trim(),
                Price = request.Price,
                Description = request.Description?.Trim() ?? string.Empty
            };
        }
    }
} 