using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs
{
    /// <summary>
    /// Request model for creating a new product
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// The name of the product
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// The price of the product
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        /// <summary>
        /// The description of the product
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for updating an existing product
    /// </summary>
    public class UpdateProductRequest
    {
        /// <summary>
        /// The updated name of the product
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// The updated price of the product
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        /// <summary>
        /// The updated description of the product
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for operations requiring a product ID
    /// </summary>
    public class GetProductByIdRequest
    {
        /// <summary>
        /// The unique identifier of the product
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer")]
        public int Id { get; set; }
    }

    /// <summary>
    /// Response model for product operations
    /// </summary>
    public class ProductResponse
    {
        /// <summary>
        /// The unique identifier of the product
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// The price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The description of the product
        /// </summary>
        public string Description { get; set; } = default!;
    }
} 